﻿function renderSplitChart(canvasId, speedValues, labels, hrValues, elevationValues, measurement) {
    const tryRender = () => {
        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            setTimeout(tryRender, 100);
            return;
        }

        if (window[canvasId + "Instance"]) {
            window[canvasId + "Instance"].destroy();
        }

        const ctx = canvas.getContext('2d');

        // Convert speed (m/s) to pace (seconds per km) or speed (km/h)
        let mainData;
        let labelMain;
        if (measurement === "pace") {
            mainData = speedValues.map(s => s > 0 ? 1000 / s : 0); // pace in seconds/km
            labelMain = "Pace (min/km)";
        } else {
            mainData = speedValues.map(s => s * 3.6);
            labelMain = "Speed (km/h)";
        }

        window[canvasId + "Instance"] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: labelMain,
                        data: mainData,
                        backgroundColor: 'rgba(54, 162, 235, 0.7)',
                        borderWidth: 1,
                        maxBarThickness: 30,
                        borderRadius: 4,
                    },
                ],
            },
            options: {
                maintainAspectRatio: false,
                indexAxis: 'y',
                elements: {
                    bar: {
                        maxBarThickness: 30,
                        borderRadius: 4,
                    }
                },
                scales: {
                    x: {
                        max: Math.max(...mainData) * 1.4,
                        beginAtZero: true,
                        grace: '10%',
                        title: {
                            display: true,
                            text: measurement === "pace" ? 'Pace (min/km)' : 'Speed (km/h)',
                        },
                        ticks: {
                            callback: function (value) {
                                if (measurement === "pace") {
                                    const minutes = Math.floor(value / 60);
                                    const seconds = Math.round(value % 60);
                                    return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
                                } else {
                                    return value.toFixed(1);
                                }
                            }
                        }
                    },
                },
                plugins: {
                    tooltip: {
                        callbacks: {
                            label: function (context) {
                                const datasetLabel = context.dataset.label || '';
                                const value = context.parsed.x;
                                if (datasetLabel === labelMain) {
                                    if (measurement === "pace") {
                                        if (value <= 0) return `${datasetLabel}: N/A`;
                                        const minutes = Math.floor(value / 60);
                                        const seconds = Math.round(value % 60);
                                        return `${datasetLabel}: ${minutes}:${seconds < 10 ? '0' : ''}${seconds} min/km`;
                                    } else {
                                        return `${datasetLabel}: ${value.toFixed(2)} km/h`;
                                    }
                                }
                                return `${datasetLabel}: ${value}`;
                            }
                        }
                    }
                }
            },
        });
    };

    tryRender();
}

window.renderLineChartById = (id, labels, data, label, chartType, minSpeed = 1) => {
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
            const minPaceSecPerKm = 1000 / minSpeed;

            const maxPace = Math.max(...data.filter(v => !isNaN(v)));

            // Clamp pace values for chart rendering
            const clampedData = data.map(v => {
                if (isNaN(v)) return NaN;
                return v > minPaceSecPerKm ? minPaceSecPerKm : v;
            });

            chartData = clampedData.map(v => isNaN(v) ? NaN : maxPace - v);

            const paceRange = Math.max(...clampedData) - Math.min(...clampedData);
            stepSize = paceRange > 300 ? 60 : 30;

            yTicksCallback = (value) => {
                const originalPace = maxPace - value;
                if (isNaN(originalPace)) return '';

                if (originalPace > minPaceSecPerKm) {
                    const minutes = Math.floor(minPaceSecPerKm / 60);
                    const seconds = Math.floor(minPaceSecPerKm % 60);
                    return `<${minutes}:${seconds.toString().padStart(2, '0')}`;
                }

                const roundedPace = Math.round(originalPace / 30) * 30;
                const minutes = Math.floor(roundedPace / 60);
                const seconds = roundedPace % 60;
                return `${minutes}:${seconds.toString().padStart(2, '0')}`;
            };

            tooltipLabelCallback = (context) => {
                // Use the original data at this point (context.dataIndex)
                const originalValue = data[context.dataIndex];
                if (isNaN(originalValue)) return '';

                if (originalValue > minPaceSecPerKm) {
                    const minutes = Math.floor(minPaceSecPerKm / 60);
                    const seconds = Math.round(minPaceSecPerKm % 60);
                    return `Pace: <${minutes}:${seconds.toString().padStart(2, '0')}`;
                }

                // Otherwise show the actual pace from clamped data
                const pace = maxPace - context.parsed.y;
                const minutes = Math.floor(pace / 60);
                const seconds = Math.round(pace % 60);
                return `Pace: ${minutes}:${seconds.toString().padStart(2, '0')} min/km`;
            };
        }

        // Colors and other chartType handling unchanged
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

window.animateProgressAndCounter = (targetPercent, durationMs, dotNetRef) => {
    const progressBar = document.querySelector('.progress-bar');
    if (!progressBar) return;

    // Start with width 0% for smooth CSS transition
    progressBar.style.width = '0%';
    // Force reflow so the transition triggers properly
    void progressBar.offsetWidth;
    progressBar.style.transition = `width ${durationMs}ms ease-out`;
    progressBar.style.width = `${targetPercent}%`;

    // Immediately set percent to 0 so text updates right away
    dotNetRef.invokeMethodAsync('UpdateAnimatedPercent', 0);

    let start = null;

    function step(timestamp) {
        if (!start) start = timestamp;
        const elapsed = timestamp - start;
        const progress = Math.min(elapsed / durationMs, 1);
        const currentPercent = Math.floor(progress * targetPercent);

        dotNetRef.invokeMethodAsync('UpdateAnimatedPercent', currentPercent);

        if (progress < 1) {
            requestAnimationFrame(step);
        } else {
            dotNetRef.invokeMethodAsync('UpdateAnimatedPercent', targetPercent);
            dotNetRef.dispose();
        }
    }

    requestAnimationFrame(step);
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