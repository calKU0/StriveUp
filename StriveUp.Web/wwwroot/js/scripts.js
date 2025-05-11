window.renderLineChartById = (id, labels, data, label, maxtics) => {
    const tryRender = () => {
        const canvas = document.getElementById(id);
        if (!canvas) {
            console.warn(`Canvas with id '${id}' not found. Retrying...`);
            setTimeout(tryRender, 100); // wait and retry
            return;
        }

        const ctx = canvas.getContext("2d");
        if (!ctx) {
            console.error("Failed to get canvas context.");
            return;
        }

        // Determine the number of ticks based on screen size
        let ticksLimit;
        if (window.innerWidth >= 1200) {
            ticksLimit = 15; // PC
        } else if (window.innerWidth >= 768) {
            ticksLimit = 10; // Tablet (md)
        } else {
            ticksLimit = 5; // Mobile (sm)
        }

        new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: label,
                    data: data,
                    borderColor: 'rgba(255, 167, 192, 38)',
                    backgroundColor: 'rgba(255, 167, 38, 0.3)',
                    tension: 0.4,
                    fill: 'origin', // This makes the area under the line filled
                    tension: 0, // Straight line (no curvature)
                    pointRadius: 0 // Remove the dots
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    x: {
                        display: true,
                        ticks: {
                            autoSkip: true, // Automatically skip labels
                            maxTicksLimit: ticksLimit, // Limit the number of ticks
                            callback: function (value, index, values) {
                                return labels[index];
                            }
                        } },
                    y: { display: true }
                },
                interaction: {
                    mode: 'index', // It helps to show labels when hovering anywhere on the chart
                    intersect: false, // It shows tooltips when hovering anywhere, not just on the line
                }
            }
        });
    };

    tryRender();
};
