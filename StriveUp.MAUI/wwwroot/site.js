let map;
let trail = [];
let polyline;
let userMarker = null;

let mapboxMap;
let route = [];
let routeLine = null;
function initializeMap(lat, lng, accessToken) {
    mapboxgl.accessToken = accessToken;

    mapboxMap = new mapboxgl.Map({
        container: 'map',
        style: 'mapbox://styles/mapbox/streets-v12',
        center: [lng, lat],
        zoom: 16,
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
}

function createArrowMarker() {
    const marker = document.createElement('div');
    marker.className = 'arrow-marker';
    const arrowIcon = document.createElement('img');
    arrowIcon.src = '/images/arrow-icon.png';  // Point to your arrow icon
    arrowIcon.style.width = '16px';
    arrowIcon.style.height = '16px';
    marker.appendChild(arrowIcon);
    return marker;
}

function updateMap(lat, lng, heading, shouldTrack) {
    const lngLat = [lng, lat];

    if (userMarker) {
        userMarker.setLngLat(lngLat);

        const arrowIcon = userMarker.getElement().querySelector('img');
        if (arrowIcon) {
            arrowIcon.style.transform = `rotate(${heading}deg)`;  // Rotate the arrow
        }
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
        duration: 1000
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


window.handleSwipe = (elementId) => {
    const element = document.getElementById(elementId);
    let startY = null;

    element.addEventListener("touchstart", (e) => {
        startY = e.touches[0].clientY;
    });

    element.addEventListener("touchend", (e) => {
        const endY = e.changedTouches[0].clientY;
        const deltaY = startY - endY;

        if (deltaY > 50) {
            element.classList.add("expanded");
        } else if (deltaY < -50) {
            element.classList.remove("expanded");
        }
    });
};


window.triggerFileInputClick = function (element) {
    if (element) {
        element.click();
    }
};
