/**
 * Method that plot area
 * @param {any} data : Data
 * @param {any} control : Div ID
 * @param {any} labels : array with months names
 */
function area(data, control, x_axis, y_axis) {
    nv.addGraph(function () {
        var chart_area = nv.models.lineChart()
            .useInteractiveGuideline(true)  //We want nice looking tooltips and a guideline!            
            .x(function (d) { return d.x; })
            .y(function (d) { return d.y; }) 
            .showLegend(true)       //Show the legend, allowing users to turn on/off line series.
            .showYAxis(true)        //Show the y-axis
            .showXAxis(true)        //Show the x-axis
            ;
        chart_area.xAxis     //Chart x-axis settings
            .axisLabel(x_axis)
            .tickFormat(function (d) {
                return d3.time.format('%Y-%m-%d')(new Date(d));
            });

        chart_area.yAxis     //Chart y-axis settings
            .axisLabel(y_axis)
            .tickFormat(d3.format('0f'));

        d3.select("#" + control + " svg")    //Select the <svg> element you want to render the chart in.   
            .datum(data)         //Populate the <svg> element with chart data...
            .call(chart_area);          //Finally, render the chart!

        //Update the chart when window resizes.
        nv.utils.windowResize(function () { chart_area.update(); });
        return chart_area;
    });
}