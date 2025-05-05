let map;
let trail = [];
let polyline;
let userMarker = null;

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


function updateMap(lat, lng, heading) {
    if (!map || !userMarker) return;

    const latLng = [lat, lng];
    trail.push(latLng);
    polyline.setLatLngs(trail);

    userMarker.setLatLng(latLng);
    userMarker.setRotationAngle(heading); // Rotate marker based on the heading
}
