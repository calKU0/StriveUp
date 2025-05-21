window.renderLineChartById = (id, labels, data, label, chartType) => {
    const tryRender = () => {
        const canvas = document.getElementById(id);
        if (!canvas) {
            setTimeout(tryRender, 100);
            return;
        }

        const ctx = canvas.getContext("2d");
        let chartData = data;
        let yTicksCallback = (value) => value;
        let tooltipLabelCallback = (context) => `${label}: ${context.parsed.y}`;

        let stepSize = 10;
        let ticksLimit = window.innerWidth >= 1200 ? 15 : window.innerWidth >= 768 ? 10 : 5;

        if (chartType === "speed") {
            const maxPace = Math.max(...data.filter(v => !isNaN(v)));
            chartData = data.map(v => isNaN(v) ? NaN : maxPace - v);
            const paceRange = Math.max(...data) - Math.min(...data);
            stepSize = paceRange > 300 ? 60 : 30;

            yTicksCallback = (value) => {
                const originalPace = maxPace - value;
                if (isNaN(originalPace)) return '';
                const roundedPace = Math.round(originalPace / 30) * 30;
                const minutes = Math.floor(roundedPace / 60);
                const seconds = roundedPace % 60;
                return `${minutes}:${seconds.toString().padStart(2, '0')}`;
            };

            tooltipLabelCallback = (context) => {
                const originalPace = maxPace - context.parsed.y;
                if (isNaN(originalPace)) return '';
                const minutes = Math.floor(originalPace / 60);
                const seconds = Math.round(originalPace % 60);
                return `Pace: ${minutes}:${seconds.toString().padStart(2, '0')} min/km`;
            };
        }

        let borderColor = 'rgba(255, 167, 192, 0.8)';
        let backgroundColor = 'rgba(255, 167, 38, 0.3)';

        if (chartType === "speed") {
            borderColor = 'rgba(13, 110, 253,1)'; // blue
            backgroundColor = 'rgba(13, 110, 253, 0.5)';
        } else if (chartType === "hr") {
            borderColor = 'rgba(255, 99, 132, 1)'; // red
            backgroundColor = 'rgba(255, 99, 132, 0.5)';
        } else if (chartType === "elevation") {
            borderColor = 'rgba(139, 69, 19, 1)'; // brown
            backgroundColor = 'rgba(139, 69, 19, 0.5)';
        }

        new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: label,
                    data: chartData,
                    borderColor: borderColor,
                    backgroundColor: backgroundColor,
                    fill: 'origin',
                    tension: 0.2,
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
                            callback: yTicksCallback
                        }
                    }
                },
                plugins: {
                    tooltip: {
                        callbacks: {
                            label: tooltipLabelCallback
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

window.activityFeedObserver = null;

window.initIntersectionObserver = (element, dotNetHelper) => {
    console.log("Observer initialized");

    if (!element) {
        console.warn("Sentinel not found");
        return;
    }

    // Disconnect previous observer if exists
    if (window.activityFeedObserver) {
        window.activityFeedObserver.disconnect();
    }

    window.activityFeedObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                dotNetHelper.invokeMethodAsync('LoadMoreActivities');
            }
        });
    });

    window.activityFeedObserver.observe(element);
};


window.triggerFileInputClick = function () {
    var fileInput = document.getElementById("fileInput");
    if (fileInput) {
        fileInput.click();
    }
}

window.headerScrollHelper = {
    lastScrollTop: 0,
    threshold: 15, // Minimum scroll difference to detect direction
    init: function (dotNetHelper) {
        window.addEventListener('scroll', function () {
            let st = window.pageYOffset || document.documentElement.scrollTop;
            if (Math.abs(st - this.lastScrollTop) <= this.threshold) {
                // Ignore small scrolls
                return;
            }

            if (st > this.lastScrollTop) {
                // Scrolling down -> hide header
                dotNetHelper.invokeMethodAsync('SetHeaderVisibility', false);
            } else {
                // Scrolling up -> show header
                dotNetHelper.invokeMethodAsync('SetHeaderVisibility', true);
            }
            this.lastScrollTop = st <= 0 ? 0 : st; // For Mobile or negative scrolling
        }.bind(this), { passive: true });
    }
};
window.getTimeAgo = function (utcDateString) {
    const utcDate = new Date(utcDateString);
    const localDate = new Date(utcDate.getTime() + (utcDate.getTimezoneOffset() * 60000) * -1);
    const now = new Date();
    const diffMs = now.getTime() - localDate.getTime();

    if (diffMs < 0) return 'just now';

    const secondsAgo = Math.floor(diffMs / 1000);
    if (secondsAgo < 60) return `${secondsAgo}s ago`;
    const minutesAgo = Math.floor(secondsAgo / 60);
    if (minutesAgo < 60) return `${minutesAgo}m ago`;
    const hoursAgo = Math.floor(minutesAgo / 60);
    if (hoursAgo < 24) return `${hoursAgo}h ago`;
    const daysAgo = Math.floor(hoursAgo / 24);
    return `${daysAgo}d ago`;
};
