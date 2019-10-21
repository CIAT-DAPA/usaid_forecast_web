/**
 * Method that plot bars
 * @param {any} data : Data
 * @param {any} control : Div ID
 * @param {any} labels : array with months names
 */
function lines(data, control, y_axis, x_axis) {
    nv.addGraph(function () {
        var chart_lines = nv.models.lineChart()
            .useInteractiveGuideline(true)
            ;

        chart_lines.xAxis
            .axisLabel(x_axis)
            .tickFormat(d3.format('0f'))
            ;

        chart_lines.yAxis
            .axisLabel(y_axis)
            .tickFormat(d3.format('0f'))
            ;

        d3.select("#" + control + " svg")
            .datum(data)
            .transition().duration(500)
            .call(chart_lines)
            ;

        nv.utils.windowResize(function () { chart_lines.update(); });

        return chart_lines;
    });
}

/**
 * Method that plots all lines for historical weather
 * @param {any} data
 * @param {any} labels
 * @param {any} vars
 */
function plot_lines(data, labels, vars, y_axis) {
    for (var i = 0; i < vars.length; i++) {
        var v = vars[i];
        var ctrl = 'line_historical_' + v;
        var l_data = [];

        for (var j = 0; j < labels.length; j++) {
            l_data.push({
                key: labels[j],
                values: data.map(function (item) {
                    // Filtering by month
                    var month_data = item.monthly_Data.filter(function (item2) { return item2.month == (j + 1) })
                        .map(function (item2) {
                            // Filtering by measure
                            var measure_data = item2.data.filter(function (item3) { return item3.measure == v; });
                            if (measure_data.length > 0)
                                return measure_data.map(function (item3) { return item3.value; })[0];
                            else
                                return null;
                        });
                    if (month_data.length > 0 && month_data[0] != null)
                        return { x: item.year, y: month_data[0] };
                    else
                        return { x: item.year, y: null };
                }).filter(function (item) { return item.y != null; })
            });
        }
        lines(l_data, ctrl, y_axis[v], x_axis);
    }

}
