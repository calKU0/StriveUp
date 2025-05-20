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

        mapboxMap = new mapboxgl.Map({
            container: 'map',
            style: 'mapbox://styles/mapbox/streets-v12',
            center: [lng, lat],
            zoom: 17,
            pitch: 60,
            bearing: 0,
            antialias: true
        });
        mapboxMap.on('load', () => {
            userMarker = new mapboxgl.Marker({
                element: createArrowMarker(), 
                anchor: 'center',
            })
                .setLngLat([lng, lat])
                .addTo(mapboxMap);

            // Add empty route line source/layer
            mapboxMap.addSource('route', {
                type: 'geojson',
                data: {
                    type: 'Feature',
                    geometry: {
                        type: 'LineString',
                        coordinates: []
                    }
                }
            });
            mapboxMap.addLayer({
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

        if (mapboxMap.getSource('route')) {
            mapboxMap.getSource('route').setData({
                type: 'Feature',
                geometry: {
                    type: 'LineString',
                    coordinates: route
                }
            });
        }
    }

    mapboxMap.easeTo({
        center: lngLat,
        bearing: heading,
        duration: 750
    });
}

function clearRoute() {
    // Remove the route line from the map
    if (mapboxMap.getSource('route')) {
        mapboxMap.getSource('route').setData({
            type: 'Feature',
            geometry: {
                type: 'LineString',
                coordinates: [] 
            }
        });
    }

    if (mapboxMap.getLayer('routeLine')) {
        mapboxMap.removeLayer('routeLine');
    }

    if (mapboxMap.getSource('route')) {
        mapboxMap.removeSource('route');
    }

    route = [];
}


function scrollToTrackingSection() {
    const section = document.getElementById("tracking-section");
    if (section) {
        section.scrollIntoView({ behavior: "smooth" });
    }
}


window.renderLineChartById = (id, labels, data, label, maxtics) => {
    const tryRender = () => {
        const canvas = document.getElementById(id);
        if (!canvas) {
            console.warn(`Canvas with id '${id}' not found. Retrying...`);
            setTimeout(tryRender, 100); // wait and retry
            return;
        }

        const ctx = canvas.getContext("2d");
        if (!ctx) {
            console.error("Failed to get canvas context.");
            return;
        }

        // Determine the number of ticks based on screen size
        let ticksLimit;
        if (window.innerWidth >= 1200) {
            ticksLimit = 15; // PC
        } else if (window.innerWidth >= 768) {
            ticksLimit = 10; // Tablet (md)
        } else {
            ticksLimit = 5; // Mobile (sm)
        }

        new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: label,
                    data: data,
                    borderColor: 'rgba(255, 167, 192, 38)',
                    backgroundColor: 'rgba(255, 167, 38, 0.3)',
                    tension: 0.4,
                    fill: 'origin', // This makes the area under the line filled
                    tension: 0, // Straight line (no curvature)
                    pointRadius: 0 // Remove the dots
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    x: {
                        display: true,
                        ticks: {
                            autoSkip: true, // Automatically skip labels
                            maxTicksLimit: ticksLimit, // Limit the number of ticks
                            callback: function (value, index, values) {
                                return labels[index];
                            }
                        }
                    },
                    y: { display: true }
                },
                interaction: {
                    mode: 'index', // It helps to show labels when hovering anywhere on the chart
                    intersect: false, // It shows tooltips when hovering anywhere, not just on the line
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
window.initIntersectionObserver = (element, dotNetHelper) => {
    console.log("Observer initialized");
    if (!element) {
        console.warn("Sentinel not found");
        return;
    }

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            console.log("Observer entry:", entry);
            if (entry.isIntersecting) {
                console.log("Loading more activities...");
                dotNetHelper.invokeMethodAsync('LoadMoreActivities');
            }
        });
    });

    observer.observe(element);
};

window.triggerFileInputClick = function () {
    var fileInput = document.getElementById("fileInput");
    if (fileInput) {
        fileInput.click();
    }
}

window.headerScrollHelper = {
    lastScrollTop: 0,
    threshold: 10, // Minimum scroll difference to detect direction
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

