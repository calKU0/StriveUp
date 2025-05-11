export function renderStaticRoute(mapId, route, accessToken) {
    mapboxgl.accessToken = accessToken;
    const coords = route.map(p => [p.longitude, p.latitude]);
    const bounds = coords.reduce((b, c) => b.extend(c), new mapboxgl.LngLatBounds(coords[0], coords[0]));

    const map = new mapboxgl.Map({
        container: mapId,
        style: 'mapbox://styles/mapbox/streets-v12',
        interactive: false,
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
}
