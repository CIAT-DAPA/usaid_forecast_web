/**
 * Method that plot a donut
 * @param {any} data : Data
 * @param {any} control : Div ID
 */
function donut(data, control) {
    $("#" + control + " svg").html("");
    nv.addGraph(function () {
        var chart_donut = nv.models.pieChart()
            .x(function (d) { return d.label })
            .y(function (d) { return d.value * 100; })
            .showLabels(true)     //Display pie labels
            .labelThreshold(.05)  //Configure the minimum slice size for labels to show up
            .labelType("percent") //Configure what type of data to show in the label. Can be "key", "value" or "percent"
            .donut(true)          //Turn on Donut mode. Makes pie chart look tasty!
            .donutRatio(0.35)     //Configure how big you want the donut hole size to be.
            .showLegend(false) 
            ;

        d3.select("#" + control + " svg")
            .datum(data)
            .transition().duration(350)
            .call(chart_donut);

        nv.utils.windowResize(function () { chart_donut.update(); });

        return chart_donut;
    });
}

/**
 * Method that plots all donuts for forecast
 * @param {any} data
 * @param {any} labels
 */
function plot_donut(data, labels) {
    for (var i = 0; i < data.length; i++) {
        var d = data[i];
        var ctrl = 'pie_' + d.year + '_' + d.month;
        var d_data = [{
            "label": labels["lower"],
            "value": d.probabilities[0].lower,
            "color": "#d79d91"
        },
        {
            "label": labels["normal"],
            "value": d.probabilities[0].normal,
            "color": "#92d892"
        },
        {
            "label": labels["upper"],
            "value": d.probabilities[0].upper,
            "color": "#6bb7c7"
        }];
    donut(d_data, ctrl);
}
    
}
