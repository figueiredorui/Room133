// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var canvas = document.getElementById("canvas").getContext('2d');
var mainChart = initChart(canvas);
refreshChart();

addHistoryCharts();

setInterval(function () {
    refreshChart();
}, 60000);


function refreshChart() {

    $.get("api/temperature/history", function (data) {

        setChartData(mainChart, data);
        if (data !== null && data.length > 0) {
            var lastentry = data[data.length - 1];
            $("#temp").text(lastentry.temperature.toFixed(1));
        }

    });
}

function initChart(canvas) {

    var config = {
        type: 'line',
        data: null,
        options: {
            animation: false,
            responsive: true,
            elements: {
                point: {
                    radius: 4,
                    hitRadius: 4,
                    hoverRadius: 4
                }
            },
            legend: {
                display: false,
            },
            tooltips: {
                backgroundColor: '#fff',
                titleFontColor: '#9787FF',
                displayColors: false,
                bodyFontSize: 15,
                callbacks: {
                    label: function (tooltipItems, data) {
                        return tooltipItems.yLabel + '°C';
                    }
                }
            },
            scales: {
                xAxes: [{
                    display: true,
                }],
                yAxes: [{
                    display: true,
                    position: 'right',
                    ticks: {
                        beginAtZero: true,
                        stepSize: 1,
                        stepValue: 1,
                        max: 40,
                        min: 15
                    }
                }, {
                    display: true,
                    position: 'left',
                    ticks: {
                        beginAtZero: true,
                        stepSize: 1,
                        stepValue: 1,
                        max: 40,
                        min: 15
                    }
                }]
            }
        }
    };

    var chart = new Chart(canvas, config);
    return chart;
}

function setChartData(chart, data) {

    if (data === null || data.length === 0)
        return;

    chart.data = {
        labels: data.map(a => a.time),
        datasets: [
            {
                label: 'Temperature',
                data: data.map(a => a.temperature),
                fill: false,
                borderColor: '#9787FF',
                
            }
        ]
    };
    chart.update();
}

function addHistoryCharts() {
    getDataAndCreateCanvas(1);
}

function getDataAndCreateCanvas(i) {

    if (i > 30)
        return;

    $.get("api/temperature/history/" + i, function (data) {
        if (data !== null && data.length > 0) {
            addHistoryCanvas(i, data);
        }
        getDataAndCreateCanvas(i + 1);
    });
}

function addDays(date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
}

function addHistoryCanvas(i, data) {
    var elementID = 'historycanvas' + i; // Unique ID

    var currentDate = new Date();
    var pastDate = addDays(currentDate, -i);

    $('<h4>')
        .text(pastDate.toDateString())
        .appendTo('#historyCharts');

    $('<canvas>')
        .attr({ id: elementID })
        .css('cssText', 'height:550px; width:100%!important')
        .appendTo('#historyCharts');

    var c = document.getElementById(elementID);

    var historyChart = initChart(c);
    setChartData(historyChart, data);

}