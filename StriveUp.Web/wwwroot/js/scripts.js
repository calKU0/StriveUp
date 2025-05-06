let map;
let trail = [];
let polyline;
let userMarker = null;

function initializeMap(lat, lng) {
    const latLng = [lat, lng];

    map = L.map("map").setView(latLng, 16);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19
    }).addTo(map);

    polyline = L.polyline([], { color: 'blue' }).addTo(map);

    userMarker = L.marker(latLng).addTo(map);

    // Setup swipe gesture for bottom sheet
    const sheet = document.getElementById('bottomSheet');
    let startY;

    sheet.addEventListener('touchstart', e => startY = e.touches[0].clientY);
    sheet.addEventListener('touchmove', e => {
        if (e.touches[0].clientY < startY) {
            sheet.classList.add('active');
        } else {
            sheet.classList.remove('active');
        }
    });
}

function updateMap(lat, lng, heading) {
    if (!map) return;

    const latLng = [lat, lng];
    trail.push(latLng);
    polyline.setLatLngs(trail);

    if (userMarker) {
        userMarker.setLatLng(latLng);
        userMarker.setRotationAngle(heading); // Optional, needs leaflet-rotatedmarker
    }
}
