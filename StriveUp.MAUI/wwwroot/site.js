let map;
let trail = [];
let polyline;
let userMarker = null;

let mapboxMap;
let route = [];
let routeLine = null;
window.initializeMap = function initializeMap(lat, lng, accessToken) {
    try {
        mapboxgl.accessToken = accessToken;

        window.map = new mapboxgl.Map({
            container: 'map',
            style: 'mapbox://styles/mapbox/streets-v12',
            center: [lng, lat],
            zoom: 17,
            pitch: 45,
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
        });
    } catch (error) {
        console.error("Error initializing map:", error);
    }
}

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
}
function createArrowMarker() {
    const marker = document.createElement('div');
    marker.className = 'arrow-marker';
    const arrowIcon = document.createElement('img');
    arrowIcon.src = '/images/icons/arrow-icon.png';  // Point to your arrow icon
    arrowIcon.style.width = '14px';
    arrowIcon.style.height = '14px';
    marker.appendChild(arrowIcon);
    return marker;
}

function updateMap(lat, lng, heading, shouldTrack) {
    const lngLat = [lng, lat];

    if (userMarker) {
        userMarker.setLngLat(lngLat);
        userMarker.getElement().querySelector('img');
    }

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

    window.map.easeTo({
        center: lngLat,
        bearing: heading,
        duration: 750,
        easing: t => t
    });
}

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
}

function scrollToTrackingSection() {
    const section = document.getElementById("tracking-section");
    if (section) {
        section.scrollIntoView({ behavior: "smooth" });
    }
}

window.renderLineChartById = (id, labels, data, label, chartType) => {
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
            const maxPace = Math.max(...data.filter(v => !isNaN(v)));
            chartData = data.map(v => isNaN(v) ? NaN : maxPace - v);
            const paceRange = Math.max(...data) - Math.min(...data);
            stepSize = paceRange > 300 ? 60 : 30;

            yTicksCallback = (value) => {
                const originalPace = maxPace - value;
                if (isNaN(originalPace)) return '';
                const roundedPace = Math.round(originalPace / 30) * 30;
                const minutes = Math.floor(roundedPace / 60);
                const seconds = roundedPace % 60;
                return `${minutes}:${seconds.toString().padStart(2, '0')}`;
            };

            tooltipLabelCallback = (context) => {
                const originalPace = maxPace - context.parsed.y;
                if (isNaN(originalPace)) return '';
                const minutes = Math.floor(originalPace / 60);
                const seconds = Math.round(originalPace % 60);
                return `Pace: ${minutes}:${seconds.toString().padStart(2, '0')} min/km`;
            };
        }

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

window.animateCounter = (dotNetRef, target, duration) => {
    let current = 0;
    const increment = target / (duration / 16); // Roughly 60fps
    const interval = setInterval(() => {
        current += increment;
        if (current >= target) {
            current = target;
            clearInterval(interval);
        }
        dotNetRef.invokeMethodAsync('UpdateAnimatedPercent', Math.round(current));
    }, 16);
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
}

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