let map;
let trail = [];
let polyline;
let userMarker = null;

let mapboxMap;
let route = [];
let routeLine = null;

function renderStaticMap(mapId, routePoints) {
    const map = L.map(mapId).setView([routePoints[0].latitude, routePoints[0].longitude], 16);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '© OpenStreetMap'
    }).addTo(map);

    const latlngs = routePoints.map(p => [p.latitude, p.longitude]);
    const polyline = L.polyline(latlngs, { color: 'dodgerblue', weight: 5 }).addTo(map);
    map.fitBounds(polyline.getBounds(), { padding: [20, 20] });
}

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


window.handleSwipe = function (sheetId) {
    const bottomSheet = document.getElementById(sheetId);
    let startY = 0;

    if (!bottomSheet) return;

    bottomSheet.addEventListener('touchstart', function (e) {
        startY = e.touches[0].clientY;
    });

    bottomSheet.addEventListener('touchend', function (e) {
        const endY = e.changedTouches[0].clientY;
        const deltaY = startY - endY;

        if (deltaY > 50) {
            bottomSheet.classList.add('expanded');
        } else if (deltaY < -50) {
            bottomSheet.classList.remove('expanded');
        }
    });

    const handle = bottomSheet.querySelector('.drag-handle');
    if (handle) {
        handle.addEventListener('click', () => {
            bottomSheet.classList.toggle('expanded');
        });
    }
};

window.triggerFileInputClick = function (element) {
    if (element) {
        element.click();
    }
};
