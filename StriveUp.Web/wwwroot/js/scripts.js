window.renderLineChartById = (id, labels, data, label) => {
    const tryRender = () => {
        const canvas = document.getElementById(id);
        if (!canvas) {
            setTimeout(tryRender, 100);
            return;
        }

        const ctx = canvas.getContext("2d");
        const maxPace = Math.max(...data.filter(v => !isNaN(v)));
        const invertedData = data.map(v => isNaN(v) ? NaN : maxPace - v);
        const paceRange = Math.max(...data) - Math.min(...data);
        const stepSize = paceRange > 300 ? 60 : 30;

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
                    data: invertedData,
                    borderColor: 'rgba(255, 167, 192, 38)',
                    backgroundColor: 'rgba(255, 167, 38, 0.3)',
                    fill: 'origin',
                    tension: 0,
                    pointRadius: 0
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    x: {
                        ticks: {
                            autoSkip: true,
                            maxTicksLimit: ticksLimit,
                            callback: (value, index) => labels[index]
                        }
                    },
                    y: {
                        ticks: {
                            stepSize: stepSize,
                            autoSkip: true,
                            maxTicksLimit: 9,
                            callback: (value) => {
                                const originalPace = maxPace - value;
                                if (isNaN(originalPace)) return '';
                                const roundedPace = Math.round(originalPace / 30) * 30;
                                const minutes = Math.floor(roundedPace / 60);
                                const seconds = roundedPace % 60;
                                return `${minutes}:${seconds.toString().padStart(2, '0')}`;
                            }
                        }
                    }
                },
                plugins: {
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                const originalPace = maxPace - context.parsed.y;
                                const roundedPace = Math.round(originalPace / 30) * 30;
                                const minutes = Math.floor(roundedPace / 60);
                                const seconds = roundedPace % 60;
                                return `Pace: ${minutes}:${seconds.toString().padStart(2, '0')} min/km`;
                            }
                        }
                    }
                },
                interaction: {
                    mode: 'index',
                    intersect: false
                }
            }
        });
    };
    tryRender();
};


window.launchConfetti = () => {
    if (window.confetti) {
        // Create a canvas if not already
        if (!window._confettiCanvas) {
            const canvas = document.createElement('canvas');
            canvas.id = 'confetti-canvas';
            canvas.style.position = 'fixed';
            canvas.style.top = 0;
            canvas.style.left = 0;
            canvas.style.width = '100%';
            canvas.style.height = '100%';
            canvas.style.pointerEvents = 'none';
            canvas.style.zIndex = 9999; // Very high to overlay modal
            document.body.appendChild(canvas);
            window._confettiCanvas = canvas;
            window.confetti = window.confetti.create(canvas, { resize: true });
        }

        // Launch confetti
        window.confetti({
            particleCount: 160,
            spread: 90,
            origin: { y: 0.6 }
        });
    }
};

// lazy loading (infinite scroll)
window.initIntersectionObserver = (element, dotNetHelper) => {
    console.log("Observer initialized");
    if (!element) {
        console.warn("Sentinel not found");
        return;
    }

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            console.log("Observer entry:", entry);
            if (entry.isIntersecting) {
                console.log("Loading more activities...");
                dotNetHelper.invokeMethodAsync('LoadMoreActivities');
            }
        });
    });

    observer.observe(element);
};
window.triggerFileInputClick = function () {
    var fileInput = document.getElementById("fileInput");
    if (fileInput) {
        fileInput.click();
    }
}