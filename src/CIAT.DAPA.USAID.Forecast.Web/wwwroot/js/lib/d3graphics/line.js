/**
 * This class draws a line graphic
 * (Base) base: Configuration to render the graphic
 */
function Line(base) {
    this.base = base;

    this.circleContainer = null;
}

/*
 * Method that render the graphic in a container
*/
Line.prototype.render = function () {
    var that = this;

    this.base.init(true, 0.5);

    // TODO code duplication check how you can avoid that
    var x = d3.time.scale().range([this.base.margin.right, this.base.width_full - this.base.margin.left]);
    var y = d3.scale.linear().range([this.base.height, 0]);

    var area = d3.svg.area()
            .interpolate('linear')
            .x(function (d) { return x(d.date); })
            .y0(this.base.height)
            .y1(function (d) { return y(d.value); });

    var line = d3.svg.line()
            //.interpolate('linear')
            .x(function (d) { return x(d.date); })
            .y(function (d) { return y(d.value); });

    // Interpolation for classes
    var classes = d3.scale.ordinal().range(that.base.classes)
                    .domain(that.base.data.raw.map(function (d) { return d.measure; }));

    var startData = this.base.data.raw.map(function (datum) {
        return {
            date: datum.date,
            value: datum.value,
            measure: datum.measure
        };
    });

    // Compute the minimum and maximum date, and the maximum value.
    x.domain(d3.extent(this.base.data.raw, function (d) { return d.date; }));
    // Compute the maximun value more 10%
    y.domain([
        d3.min(that.base.data.raw, function (d) { return d.value; }) * 0.95,
        d3.max(that.base.data.raw, function (d) { return d.value; }) * 1.05
    ]);

    // Add the axis
    this.base.addAxis(x, y, 10);

    // Nest the entries by symbol
    var dataNest = d3.nest()
        .key(function (d) { return d.measure; })
        .entries(startData);

    // Add the ticks
    this.base.addAxisTicks(x, y, that.base.data.raw.length / dataNest.length, 12);

    startData.forEach(function (d) {
        d.date = d.date;
        d.value = +d.value;
    });

    // Loop through each symbol / key
    dataNest.forEach(function (d) {
        // Add the line path.
        that.base.svg.append('g')
            .attr('class', 'line_area_line')
            .append('path')
            .attr('d', line(d.values))
            .attr('class', 'line_area_line_' + classes(d.key));

        // Add the area path.
        that.base.svg.append('g')
            .attr('class', 'line_area_area')
            .append('path')
            .attr('d', area(d.values))
            .attr('class', 'line_area_area_' + classes(d.key));
    });
    
    for (var i = 0; i < that.base.data.splitted.length; i++) {
        var lineNormal = [];
        lineNormal.push(that.base.data.splitted[i]);
        // Line Normal
        that.base.svg.append('g')
            .attr('class', 'line_area_splitted')
            .data(lineNormal)
            .append('line')
            .attr('x1', that.base.margin.right)
            .attr('y1', function (d) { return y(that.base.formats.round(d.value)); })
            .attr('x2', that.base.width_full - that.base.margin.left)
            .attr('y2', function (d) { return y(that.base.formats.round(d.value)); });

        // Text Line Normal
        that.base.svg.append('g')
            .attr('class', 'line_area_splitted_text')
            .data(lineNormal)
            .append('text')
            .attr('x', that.base.margin.right)
            .attr('y', function (d) { return y(that.base.formats.round(d.value)); })
            .text('Promedio histórico');
    }    

    // Add circles to show more details
    var circles = that.base.svg.append('g')
                    .attr("class", "line_area_point");

    circles.selectAll('.line_area_point')
        .data(that.base.data.raw)
        .enter()
        .append('circle')
        .attr('class', function (d) { return 'line_area_circle_' + classes(d.measure); })
        .attr('r', 5)
        .attr('cx', function (d) { return x(d.date); })
        .attr('cy', function (d) { return y(d.value); })
        .on("mouseover", function (d, i) {
            d3.select(this)
                .attr('class', function (d) { return 'line_area_circle_highlighted_' + classes(d.measure); });
            var content = 'Año: ' + d.year + '<br / >Valor: ' + that.base.formats.round(d.value) + ' ' + that.base.axis_labels.y;
            that.base.tooltip_show(d3.event.pageX, d3.event.pageY - 50, content);
        })
        .on("mouseout", function (d, i) {
            d3.select(this)
                .attr('class', function (d) { return 'line_area_circle_' + classes(d.measure); });
            that.base.tooltip_hide();
        });
}