let map;
let trail = [];
let polyline;
let userMarker = null;


export function renderStaticMap(mapId, routePoints) {
    const map = L.map(mapId).setView([routePoints[0].latitude, routePoints[0].longitude], 13);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution: '© OpenStreetMap'
    }).addTo(map);

    const latlngs = routePoints.map(p => [p.latitude, p.longitude]);
    const polyline = L.polyline(latlngs, { color: 'dodgerblue', weight: 5 }).addTo(map);
    map.fitBounds(polyline.getBounds(), { padding: [20, 20] });
}


function initializeMap(lat, lng) {
    map = L.map("map").setView([lat, lng], 16);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 19 }).addTo(map);

    polyline = L.polyline([], { color: 'blue' }).addTo(map);

    userMarker = L.marker([lat, lng]).addTo(map); // Add user marker on the map
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


function updateMap(lat, lng, heading) {
    if (!map || !userMarker) return;

    const latLng = [lat, lng];
    trail.push(latLng);
    polyline.setLatLngs(trail);

    userMarker.setLatLng(latLng);
}
