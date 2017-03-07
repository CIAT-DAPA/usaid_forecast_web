/**
 * This class draws a line graphic
 * (Base) base: Configuration to render the graphic
 * (object[]) ranges: Array with levels of yield standard
 * (int) year: Year to draw
 */
function CalendarHeatmap(base, ranges, year) {
    this.base = base;

    this.ranges = ranges;

    this.year = year;
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
CalendarHeatmap.prototype.cell_size = function () { return this.base.width / 53; }

/*
 * Metho to draw the limit for every month
 * (date) t0: Init date of month
 * (CalendarHeatmap) that: Current instance
*/
CalendarHeatmap.prototype.month_path = function (t0, that) {
    var t1 = new Date(t0.getFullYear(), t0.getMonth() + 1, 0),
        d0 = t0.getDay(), w0 = d3.time.weekOfYear(t0),
        d1 = t1.getDay(), w1 = d3.time.weekOfYear(t1);
    return "M" + (w0 + 1) * this.cell_size() + "," + d0 * this.cell_size()
        + "H" + w0 * this.cell_size() + "V" + 7 * this.cell_size()
        + "H" + w1 * this.cell_size() + "V" + (d1 + 1) * this.cell_size()
        + "H" + (w1 + 1) * this.cell_size() + "V" + 0
        + "H" + (w0 + 1) * this.cell_size() + "Z";
}

/*
 * Method that render the graphic in a container
*/
CalendarHeatmap.prototype.render = function () {
    var that = this;

    var element = document.getElementById(that.base.container.replace('#', ''));
    var height = (element.clientWidth / 53) * 8;
    this.base.init(false, height);

    this.base.svg
        .data(d3.range(that.year, that.year + 1))
        .enter();
    /*    .attr("width", '100%')
        .attr("data-height", '0.5678')
        .attr("viewBox", '0 0 900 105')
        .attr("class", "RdYlGn")
        .append("g")
        .attr("transform", "translate(" + ((width - cellSize * 53) / 2) + "," + (height - cellSize * 7 - 1) + ")");

    this.base.svg.append("text")
        .attr("transform", "translate(-38," + that.cell_size() * 3.5 + ")rotate(-90)")
        .style("text-anchor", "middle")
        .text(function (d) { return d; });*/

    // Add the days names
    this.base.svg.append("g")
        .attr("class", "days_names")
        .attr("transform", function (i) { return "translate(0," + that.base.margin.top + ")"; })
        .selectAll(".days_names")
        .data([0, 1, 2, 3, 4, 5, 6])
        .enter()
        .append("text")
        .attr("transform", function (i) { return "translate(0," + that.cell_size() * (i + 1) + ")"; })
        //.style("text-anchor", "end")
        //.attr("dy", "-.25em")
        .text(function (d) { return that.base.days_names[d].substring(0,3) + "."; });

    // Add the month names
    this.base.svg.append("g")
        .attr("class", "month_names")
        .attr("transform", function (i) { return "translate(" + that.base.margin.left + ",0)"; })
        .selectAll(".month_names")
        .data(that.base.month_names)
        .enter()
        .append("text")
        .attr("transform", function (d, i) { return "translate(" + ((i+1) * (that.cell_size() * 4)) + ",0)"; })
        .style("text-anchor", "end")
        .attr("dy", ".9em")
        .text(function (d, i) { return that.base.month_names[i] });
        

    // Create the rects of every day of year
    var rect = this.base.svg.append("g")
        .attr("class", "rect_days")
        .selectAll(".rect_days")
        .data(function (d) { return d3.time.days(new Date(d, 0, 1), new Date(d + 1, 0, 1)); })
        .enter()
        .append("rect")
        .attr("class", "heatmap_day")
        .attr("width", that.cell_size())
        .attr("height", that.cell_size())
        .attr("x", function (d) { return (d3.time.weekOfYear(d) * that.cell_size()); })
        .attr("y", function (d) { return (d.getDay() * that.cell_size()); })
        .datum(that.base.formats.date_format)
        .attr("transform", function (i) { return "translate(" + that.base.margin.left + "," + that.base.margin.top + ")"; });

    // Create the region for every month
    this.base.svg.append("g")
        .attr("class", "month_path")
        .selectAll(".month_path")
        .data(function (d) { return d3.time.months(new Date(d, 0, 1), new Date(d + 1, 0, 1)); })
        .enter()
        .append("path")
        .attr("class", "heatmap_month")
        .attr("id", function (d, i) { return that.base.month_names[i] })
        .attr("d", function (d) { return that.month_path(d, that); })
        .attr("transform", function (i) { return "translate(" + that.base.margin.left + "," + that.base.margin.top + ")"; });
        

    var data = d3.nest()
        .key(function (d) { return d.date; })
        .rollup(function (d) { return d[0].avg; })
        .map(that.base.data);

    rect.filter(function (d) { return d in data; })
        .attr("class", function (d) { return that.color(data[d]); })
        .attr("fill", function (d) { return that.color(data[d]); })
        .select("title")
        .text(function (d) { return d + ": " + round(data[d]); });

    rect.on("mouseover", function (d) {
        var value = that.base.formats.round((data[d] !== undefined) ? data[d] : 0) + ' Kg/ha';
        var content = d + ": " + value;
        that.base.tooltip_show(d3.event.pageX + 20, d3.event.pageY, content);        
    });

    rect.on("mouseout", function (d) {
        that.base.tooltip_hide();
    });
}