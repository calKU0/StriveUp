let map;

function initializeMap(elementId) {
    // Ensure the map container exists
    const container = document.getElementById(elementId);
    if (!container) {
        console.error("Map container not found.");
        return;
    }

    // Initialize the map with the given container
    map = L.map(elementId).setView([51.505, -0.09], 13);  // Default location: Latitude, Longitude (London)

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(map);
}

function updateMap(latitude, longitude) {
    if (map) {
        map.setView([latitude, longitude], 13);  // Move the map to the new position
        L.marker([latitude, longitude]).addTo(map)
            .bindPopup("You are here")
            .openPopup();
    }
}
