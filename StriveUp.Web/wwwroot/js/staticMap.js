export function renderStaticRoute(mapId, route, accessToken, isInteractive) {
    mapboxgl.accessToken = accessToken;
    const coords = route.map(p => [p.longitude, p.latitude]);
    const bounds = coords.reduce((b, c) => b.extend(c), new mapboxgl.LngLatBounds(coords[0], coords[0]));

    const map = new mapboxgl.Map({
        container: mapId,
        style: 'mapbox://styles/mapbox/streets-v12',
        interactive: isInteractive,
        bounds: bounds,
        fitBoundsOptions: { padding: 60 },
        attributionControl: false
    });

    map.on('load', () => {
        map.addSource('route', {
            type: 'geojson',
            data: {
                type: 'Feature',
                geometry: {
                    type: 'LineString',
                    coordinates: coords
                }
            }
        });

        map.addLayer({
            id: 'routeLine',
            type: 'line',
            source: 'route',
            paint: {
                'line-color': '#FF4500',
                'line-width': 4
            }
        });

        // Add start marker
        new mapboxgl.Marker({
            color: "#285A98" // Blue
        })
            .setLngLat(coords[0])
            .setPopup(new mapboxgl.Popup().setText("Start")) // Optional
            .addTo(map);

        // Add end marker
        new mapboxgl.Marker({
            color: "#FF5722" // Orange
        })
            .setLngLat(coords[coords.length - 1])
            .setPopup(new mapboxgl.Popup().setText("Finish")) // Optional
            .addTo(map);
    });

    //new mapboxgl.Marker({
    //    element: createCustomMarker('https://yourdomain.com/icons/start.png')
    //})
    //    .setLngLat(coords[0])
    //    .addTo(map);

    window[mapId] = map; 
    return map; 
}

export function resizeMap(mapId) {
    const map = window[mapId];
    if (map) {
        const container = document.getElementById(mapId);
        container.style.width = '100%'; // Reflow by changing width temporarily

        let elapsedTime = 0;
        const intervalId = setInterval(() => {
            map.resize(); // Resize map
            elapsedTime += 10; // Increment by 10ms

            if (elapsedTime >= 300) {
                clearInterval(intervalId); // Stop after 300ms
            }
        }, 10); // 10ms interval
    }
}

// helper
function createCustomMarker(iconUrl) {
    const img = document.createElement('img');
    img.src = iconUrl;
    img.style.width = '32px';
    img.style.height = '32px';
    img.style.transform = 'translate(-50%, -100%)';
    return img;
}

