/**
 * This class draws a line graphic
 * (Base) base: Configuration to render the graphic
 * (object[]) ranges: Array with levels of yield standard
 */
function CalendarHeatmap(base, ranges) {
    this.base = base;

    this.ranges = ranges;

}

/*
 * Method that return a function to interpolate the values to color
*/
CalendarHeatmap.prototype.color = function (value) {
    var domain = this.ranges.treashold.map(function (d) { return d + 1; });
    var range = ['#ad5858', '#ad7e58', '#abad58', '#8fad58', '#69ad58'];
    var color = d3.scale.threshold().domain(domain).range(range);
    return color(value);
}

/*
 * Get the size of a cell in the graphic
*/
CalendarHeatmap.prototype.cell_size = function () { return 12; }

/*
 * Method that render the graphic in a container
*/
CalendarHeatmap.prototype.render = function () {
    var that = this;

    this.base.init(false, 105);

    this.base.svg
        .data(d3.range(2016, 2017))
        .enter();
    /*    .attr("width", '100%')
        .attr("data-height", '0.5678')
        .attr("viewBox", '0 0 900 105')
        .attr("class", "RdYlGn")
        .append("g")
        .attr("transform", "translate(" + ((width - cellSize * 53) / 2) + "," + (height - cellSize * 7 - 1) + ")");*/

    this.base.svg.append("text")
        .attr("transform", "translate(-38," + that.cell_size() * 3.5 + ")rotate(-90)")
        .style("text-anchor", "middle")
        .text(function (d) { return d; });

    for (var i = 0; i < 7; i++) {
        this.base.svg.append("text")
            .attr("transform", "translate(-5," + that.cell_size() * (i + 1) + ")")
            .style("text-anchor", "end")
            .attr("dy", "-.25em")
            .text(function (d) { return that.base.days_names[i]; });
    }

    // Create the rect
    var rect = this.base.svg.append("g")
        .attr("class", "rect_days")
        .selectAll(".day")
        .data(function (d) { return d3.time.days(new Date(d, 0, 1), new Date(d + 1, 0, 1)); })
        .enter()
        .append("rect")
        .attr("class", "day")
        .attr("width", that.cell_size())
        .attr("height", that.cell_size())
        .attr("x", function (d) { return d3.time.weekOfYear(d) * that.cell_size(); })
        .attr("y", function (d) { return d.getDay() * that.cell_size(); })
        .datum(that.base.formats.date);

    var legend = this.base.svg.append("g")
        .attr("class", "month_names")
        .selectAll(".legend")
        .data(that.base.month_names)
        .enter().append("g")
        .attr("class", "legend")
        .attr("transform", function (d, i) { return "translate(" + (((i + 1) * 50) + 8) + ",0)"; });

    legend.append("text")
        .attr("class", function (d, i) { return that.base.month_names[i] })
        .style("text-anchor", "end")
        .attr("dy", "-.25em")
        .text(function (d, i) { return that.base.month_names[i] });

    this.base.svg.selectAll(".month")
        .data(function (d) { return d3.time.months(new Date(d, 0, 1), new Date(d + 1, 0, 1)); })
        .enter().append("path")
        .attr("class", "month")
        .attr("id", function (d, i) { return that.base.month_names[i] })
        .attr("d", monthPath);

    /*var data = d3.nest()
        .key(function (d) { return d.Fecha; })
        .rollup(function (d) { return d[0].RendimientoPromedio; })
        .map(items);

    rect.filter(function (d) { return d in data; })
        .attr("class", function (d) { return "day " + that.color(data[d]); })
        .select("title")
        .text(function (d) { return d + ": " + round(data[d]); });

    //  Tooltip Object
    /*var tooltip = d3.select("body")
        .append("div").attr("id", "tooltip")
        .style("position", "absolute")
        .style("z-index", "10")
        .style("visibility", "hidden")
        .text("a simple tooltip");

    rect.on("mouseover", function (d) {
        tooltip.style("visibility", "visible");
        var value = ((data[d] !== undefined) ? round(data[d]) : round(0)) + ' Kg/ha';
        var purchase_text = d + ": " + value;

        tooltip.transition()
            .duration(200)
            .style("opacity", .9);
        tooltip.html(purchase_text)
            .style("left", (d3.event.pageX) + 30 + "px")
            .style("top", (d3.event.pageY) + "px");
    });

    rect.on("mouseout", function (d) {
        tooltip.transition()
            .duration(500)
            .style("opacity", 0);
        var $tooltip = $("#tooltip");
        $tooltip.empty();
    });*/

    function monthPath(t0) {
        var t1 = new Date(t0.getFullYear(), t0.getMonth() + 1, 0),
            d0 = t0.getDay(), w0 = d3.time.weekOfYear(t0),
            d1 = t1.getDay(), w1 = d3.time.weekOfYear(t1);
        return "M" + (w0 + 1) * cellSize + "," + d0 * cellSize
            + "H" + w0 * cellSize + "V" + 7 * cellSize
            + "H" + w1 * cellSize + "V" + (d1 + 1) * cellSize
            + "H" + (w1 + 1) * cellSize + "V" + 0
            + "H" + (w0 + 1) * cellSize + "Z";
    }
}