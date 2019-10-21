/**
 * Method that plot bars
 * @param {any} data : Data
 * @param {any} control : Div ID
 * @param {any} labels : array with months names
 */
function bars(data, control, y_axis, rotate) {
    nv.addGraph(function () {
        var chart_bar = nv.models.discreteBarChart()
            .x(function (d) { return d.label })    //Specify the data accessors.
            .y(function (d) { return d.value })
            .staggerLabels(false)    //Too many bars and not enough room? Try staggering labels.                        
            .showValues(false)       //...instead, show the bar value right on top of each bar.                        
            .rotateLabels(rotate ? -45 : 0)      //Angle to rotate x-axis labels.                        
            ;

        chart_bar.yAxis
            .axisLabel(y_axis)
            .tickFormat(d3.format('0f'))
            ;

        d3.select("#" + control + " svg")
            .datum(data)
            .call(chart_bar);

        nv.utils.windowResize(function () { chart_bar.update(); });

        return chart_bar;
    });
}

/**
 * Method that plots all bars for climatology
 * @param {any} data
 * @param {any} labels
 * @param {any} vars
 */
function plot_bars(data, labels, vars, y_axis) {
    var colors = { prec: '#6bb7c7', sol_rad: '#c7966b', t_max: '#c76b6b', t_min: '#c7c76b'};
    for (var i = 0; i < vars.length; i++) {
        var v = vars[i];
        var ctrl = 'bar_climatology_' + v;
        var b_data = [{
            key: v,
            values: data.map(function (item) {
                return {
                    "label": labels[item.month - 1].substring(0, 3),
                    "value": item.data.filter(function (item2) { return item2.measure === v; })[0].value,
                    "color": colors[v]
                };
            })
        }];
        bars(b_data, ctrl, y_axis[v], false);
    }

}
