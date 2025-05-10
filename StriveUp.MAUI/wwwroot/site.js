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

        const arrowIcon = userMarker.getElement().querySelector('img');
        if (arrowIcon) {
            arrowIcon.style.transform = `rotate(${heading - 90}deg)`;  // Rotate the arrow
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


window.triggerFileInputClick = function (element) {
    if (element) {
        element.click();
    }
};
