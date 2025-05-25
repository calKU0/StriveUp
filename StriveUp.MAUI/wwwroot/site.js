let map;
let trail = [];
let polyline;
let userMarker = null;
let isUserInteracting = false;

let mapboxMap;
let route = [];
let routeLine = null;
let cumulativeHeading = 0;
let lastHeading = 0;

window.headerScrollHelper = {
    lastScrollTop: 0,
    threshold: 15, // Minimum scroll difference to detect direction
    init: function (dotNetHelper) {
        window.addEventListener('scroll', function () {
            let st = window.pageYOffset || document.documentElement.scrollTop;
            if (Math.abs(st - this.lastScrollTop) <= this.threshold) {
                // Ignore small scrolls
                return;
            }

            if (st > this.lastScrollTop) {
                // Scrolling down -> hide header
                dotNetHelper.invokeMethodAsync('SetHeaderVisibility', false);
            } else {
                // Scrolling up -> show header
                dotNetHelper.invokeMethodAsync('SetHeaderVisibility', true);
            }
            this.lastScrollTop = st <= 0 ? 0 : st; // For Mobile or negative scrolling
        }.bind(this), { passive: true });
    }
};
window.initializeMap = function initializeMap(lat, lng, accessToken) {
    try {
        mapboxgl.accessToken = accessToken;

        window.map = new mapboxgl.Map({
            container: 'map',
            style: 'mapbox://styles/mapbox/streets-v12',
            center: [lng, lat],
            zoom: 17,
            pitch: 0,
            bearing: 0,
            antialias: true
        });
        window.map.on('load', () => {
            userMarker = new mapboxgl.Marker({
                element: createArrowMarker(),
                anchor: 'center',
            })
                .setLngLat([lng, lat])
                .addTo(window.map);

            // Add empty route line source/layer
            window.map.addSource('route', {
                type: 'geojson',
                data: {
                    type: 'Feature',
                    geometry: {
                        type: 'LineString',
                        coordinates: []
                    }
                }
            });
            window.map.addLayer({
                id: 'routeLine',
                type: 'line',
                source: 'route',
                layout: {
                    'line-join': 'round',
                    'line-cap': 'round'
                },
                paint: {
                    'line-color': '#1E90FF',
                    'line-width': 4
                }
            });
            window.map.on('dragstart', () => {
                isUserInteracting = true;
            });
            addCenterButton();
        });
    } catch (error) {
        console.error("Error initializing map:", error);
    }
};

function createArrowMarker() {
    const marker = document.createElement('div');
    marker.className = 'arrow-marker';
    const arrowIcon = document.createElement('img');
    arrowIcon.src = '/images/icons/arrow-icon.png';  // Point to your arrow icon
    arrowIcon.style.width = '14px';
    arrowIcon.style.height = '14px';
    marker.appendChild(arrowIcon);
    return marker;
};

function addCenterButton() {
    const button = document.createElement('button');
    button.textContent = '📍';
    button.className = 'center-button';
    button.style.position = 'absolute';
    button.style.top = '10px';
    button.style.right = '10px';
    button.style.zIndex = 1;
    button.style.background = 'white';
    button.style.border = 'none';
    button.style.borderRadius = '4px';
    button.style.padding = '6px';

    button.onclick = () => {
        isUserInteracting = false;
        if (window.currentLocation) {
            updateMap(window.currentLocation.lat, window.currentLocation.lng, false, true);
        }
    };

    document.getElementById('map').appendChild(button);
};

function updateMap(lat, lng, shouldTrack, forceCenter = false) {
    const lngLat = [lng, lat];
    window.currentLocation = { lat, lng };

    userMarker.setLngLat(lngLat);

    if (shouldTrack) {
        route.push(lngLat);
        if (window.map.getSource('route')) {
            window.map.getSource('route').setData({
                type: 'Feature',
                geometry: {
                    type: 'LineString',
                    coordinates: route
                }
            });
        }
    }

    if (!isUserInteracting || forceCenter) {
        window.map.easeTo({
            center: lngLat,
            duration: 750,
            easing: t => t
        });
    }
};

function updateMarker(heading) {
    if (!userMarker) return;

    // Normalize heading to 0-360
    heading = ((heading % 360) + 360) % 360;

    // Calculate difference
    let diff = heading - lastHeading;

    // Fix wrap-around so diff is smallest path
    if (diff < -180) diff += 360;
    else if (diff > 180) diff -= 360;

    // Update cumulative heading by adding difference
    cumulativeHeading += diff;

    // Apply rotation using cumulative heading
    const img = userMarker.getElement().querySelector('img');
    if (img) {
        img.style.transform = `rotate(${cumulativeHeading}deg)`;
    }

    lastHeading = heading;
};

function interpolateAngle(startAngle, endAngle, t) {
    // Normalize angles between 0 and 360
    startAngle = startAngle % 360;
    endAngle = endAngle % 360;

    let delta = endAngle - startAngle;

    if (delta > 180) {
        delta -= 360;  // rotate backward the shorter way
    } else if (delta < -180) {
        delta += 360;  // rotate forward the shorter way
    }

    let result = startAngle + delta * t;

    return (result + 360) % 360;  // normalize again
};

function resizeMap() {
    const map = window.map;

    if (map) {
        // Resize map after layout
        requestAnimationFrame(() => {
            setTimeout(() => {
                map.resize();
            }, 50);
        });
    } else {
        console.warn('Map or container not found');
    }
};

function clearRoute() {
    // Remove the route line from the map
    if (window.map.getSource('route')) {
        window.map.getSource('route').setData({
            type: 'Feature',
            geometry: {
                type: 'LineString',
                coordinates: []
            }
        });
    }

    if (window.map.getLayer('routeLine')) {
        window.map.removeLayer('routeLine');
    }

    if (window.map.getSource('route')) {
        window.map.removeSource('route');
    }

    route = [];
};

function scrollToTrackingSection() {
    const section = document.getElementById("tracking-section");
    if (section) {
        section.scrollIntoView({ behavior: "smooth" });
    }
};

function renderSplitChart(canvasId, speedValues, labels, hrValues, elevationValues, measurement) {
    const tryRender = () => {
        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            setTimeout(tryRender, 100);
            return;
        }

        if (window[canvasId + "Instance"]) {
            window[canvasId + "Instance"].destroy();
        }

        const ctx = canvas.getContext('2d');

        // Convert speed (m/s) to pace (seconds per km) or speed (km/h)
        let mainData;
        let labelMain;
        if (measurement === "pace") {
            mainData = speedValues.map(s => s > 0 ? 1000 / s : 0); // pace in seconds/km
            labelMain = "Pace (min/km)";
        } else {
            mainData = speedValues.map(s => s * 3.6);
            labelMain = "Speed (km/h)";
        }

        window[canvasId + "Instance"] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: labelMain,
                        data: mainData,
                        backgroundColor: 'rgba(54, 162, 235, 0.7)',
                        borderWidth: 1,
                        maxBarThickness: 30,
                        borderRadius: 4,
                    },
                ],
            },
            options: {
                maintainAspectRatio: false,
                indexAxis: 'y',
                elements: {
                    bar: {
                        maxBarThickness: 30,
                        borderRadius: 4,
                    }
                },
                scales: {
                    x: {
                        max: Math.max(...mainData) * 1.4,
                        beginAtZero: true,
                        grace: '10%',
                        title: {
                            display: true,
                            text: measurement === "pace" ? 'Pace (min/km)' : 'Speed (km/h)',
                        },
                        ticks: {
                            callback: function (value) {
                                if (measurement === "pace") {
                                    const minutes = Math.floor(value / 60);
                                    const seconds = Math.round(value % 60);
                                    return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
                                } else {
                                    return value.toFixed(1);
                                }
                            }
                        }
                    },
                },
                plugins: {
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                const datasetLabel = context.dataset.label || '';
                                const value = context.parsed.x;
                                if (datasetLabel === labelMain) {
                                    if (measurement === "pace") {
                                        if (value <= 0) return `${datasetLabel}: N/A`;
                                        const minutes = Math.floor(value / 60);
                                        const seconds = Math.round(value % 60);
                                        return `${datasetLabel}: ${minutes}:${seconds < 10 ? '0' : ''}${seconds} min/km`;
                                    } else {
                                        return `${datasetLabel}: ${value.toFixed(2)} km/h`;
                                    }
                                }
                                return `${datasetLabel}: ${value}`;
                            }
                        }
                    }
                }
            },
        });
    };

    tryRender();
};

window.renderLineChartById = (id, labels, data, label, chartType, minSpeed = 1) => {
    const tryRender = () => {
        const canvas = document.getElementById(id);
        if (!canvas) {
            setTimeout(tryRender, 100);
            return;
        }

        const ctx = canvas.getContext("2d");
        let chartData = data;
        let yTicksCallback = (value) => value;
        let tooltipLabelCallback = (context) => `${label}: ${context.parsed.y}`;

        let stepSize = 10;
        let ticksLimit = window.innerWidth >= 1200 ? 15 : window.innerWidth >= 768 ? 10 : 5;

        if (chartType === "speed") {
            const minPaceSecPerKm = 1000 / minSpeed;

            const maxPace = Math.max(...data.filter(v => !isNaN(v)));

            // Clamp pace values for chart rendering
            const clampedData = data.map(v => {
                if (isNaN(v)) return NaN;
                return v > minPaceSecPerKm ? minPaceSecPerKm : v;
            });

            chartData = clampedData.map(v => isNaN(v) ? NaN : maxPace - v);

            const paceRange = Math.max(...clampedData) - Math.min(...clampedData);
            stepSize = paceRange > 300 ? 60 : 30;

            yTicksCallback = (value) => {
                const originalPace = maxPace - value;
                if (isNaN(originalPace)) return '';

                if (originalPace > minPaceSecPerKm) {
                    const minutes = Math.floor(minPaceSecPerKm / 60);
                    const seconds = Math.floor(minPaceSecPerKm % 60);
                    return `<${minutes}:${seconds.toString().padStart(2, '0')}`;
                }

                const roundedPace = Math.round(originalPace / 30) * 30;
                const minutes = Math.floor(roundedPace / 60);
                const seconds = roundedPace % 60;
                return `${minutes}:${seconds.toString().padStart(2, '0')}`;
            };

            tooltipLabelCallback = (context) => {
                // Use the original data at this point (context.dataIndex)
                const originalValue = data[context.dataIndex];
                if (isNaN(originalValue)) return '';

                if (originalValue > minPaceSecPerKm) {
                    const minutes = Math.floor(minPaceSecPerKm / 60);
                    const seconds = Math.round(minPaceSecPerKm % 60);
                    return `Pace: <${minutes}:${seconds.toString().padStart(2, '0')}`;
                }

                // Otherwise show the actual pace from clamped data
                const pace = maxPace - context.parsed.y;
                const minutes = Math.floor(pace / 60);
                const seconds = Math.round(pace % 60);
                return `Pace: ${minutes}:${seconds.toString().padStart(2, '0')} min/km`;
            };
        }

        // Colors and other chartType handling unchanged
        let borderColor = 'rgba(255, 167, 192, 0.8)';
        let backgroundColor = 'rgba(255, 167, 38, 0.3)';

        if (chartType === "speed") {
            borderColor = 'rgba(13, 110, 253,1)'; // blue
            backgroundColor = 'rgba(13, 110, 253, 0.5)';
        } else if (chartType === "hr") {
            borderColor = 'rgba(255, 99, 132, 1)'; // red
            backgroundColor = 'rgba(255, 99, 132, 0.5)';
        } else if (chartType === "elevation") {
            borderColor = 'rgba(139, 69, 19, 1)'; // brown
            backgroundColor = 'rgba(139, 69, 19, 0.5)';
        }

        new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: label,
                    data: chartData,
                    borderColor: borderColor,
                    backgroundColor: backgroundColor,
                    fill: 'origin',
                    tension: 0.2,
                    pointRadius: 0
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    x: {
                        ticks: {
                            autoSkip: true,
                            maxTicksLimit: ticksLimit,
                            callback: (value, index) => labels[index]
                        }
                    },
                    y: {
                        ticks: {
                            stepSize: stepSize,
                            autoSkip: true,
                            maxTicksLimit: 9,
                            callback: yTicksCallback
                        }
                    }
                },
                plugins: {
                    tooltip: {
                        callbacks: {
                            label: tooltipLabelCallback
                        }
                    }
                },
                interaction: {
                    mode: 'index',
                    intersect: false
                }
            }
        });
    };
    tryRender();
};

window.launchConfetti = () => {
    if (window.confetti) {
        // Create a canvas if not already
        if (!window._confettiCanvas) {
            const canvas = document.createElement('canvas');
            canvas.id = 'confetti-canvas';
            canvas.style.position = 'fixed';
            canvas.style.top = 0;
            canvas.style.left = 0;
            canvas.style.width = '100%';
            canvas.style.height = '100%';
            canvas.style.pointerEvents = 'none';
            canvas.style.zIndex = 9999; // Very high to overlay modal
            document.body.appendChild(canvas);
            window._confettiCanvas = canvas;
            window.confetti = window.confetti.create(canvas, { resize: true });
        }

        // Launch confetti
        window.confetti({
            particleCount: 160,
            spread: 90,
            origin: { y: 0.6 }
        });
    }
};

// lazy loading (infinite scroll)
window.activityFeedObserver = null;

window.initIntersectionObserver = (element, dotNetHelper) => {
    console.log("Observer initialized");

    if (!element) {
        console.warn("Sentinel not found");
        return;
    }

    // Disconnect previous observer if exists
    if (window.activityFeedObserver) {
        window.activityFeedObserver.disconnect();
    }

    window.activityFeedObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                dotNetHelper.invokeMethodAsync('LoadMoreActivities');
            }
        });
    });

    window.activityFeedObserver.observe(element);
};

window.triggerFileInputClick = function () {
    var fileInput = document.getElementById("fileInput");
    if (fileInput) {
        fileInput.click();
    }
};


window.getTimeAgo = function (utcDateString) {
    const utcDate = new Date(utcDateString);
    const localDate = new Date(utcDate.getTime() + (utcDate.getTimezoneOffset() * 60000) * -1);
    const now = new Date();
    const diffMs = now.getTime() - localDate.getTime();

    if (diffMs < 0) return 'just now';

    const secondsAgo = Math.floor(diffMs / 1000);
    if (secondsAgo < 60) return `${secondsAgo}s ago`;
    const minutesAgo = Math.floor(secondsAgo / 60);
    if (minutesAgo < 60) return `${minutesAgo}m ago`;
    const hoursAgo = Math.floor(minutesAgo / 60);
    if (hoursAgo < 24) return `${hoursAgo}h ago`;
    const daysAgo = Math.floor(hoursAgo / 24);
    return `${daysAgo}d ago`;
};