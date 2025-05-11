export function renderStaticRoute(mapId, route, accessToken, isInteractive) {
    mapboxgl.accessToken = accessToken;
    const coords = route.map(p => [p.longitude, p.latitude]);
    const bounds = coords.reduce((b, c) => b.extend(c), new mapboxgl.LngLatBounds(coords[0], coords[0]));

    const map = new mapboxgl.Map({
        container: mapId,
        style: 'mapbox://styles/mapbox/streets-v12',
        interactive: isInteractive,
        bounds: bounds,
        fitBoundsOptions: { padding: 30 },
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
    });

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

