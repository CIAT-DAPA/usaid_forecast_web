﻿/**
 * This class draws a trend graphic
 * (Base) base: Configuration to render the graphic
 */
function Trend(base) {
    this.base = base;
 }


Trend.prototype.render = function () {    
    var that = this;
    that.base.init(true, 0.5);

    // Interpolation function
    var x = d3.time.scale().range([that.base.margin.right, that.base.width_full - that.base.margin.left])
            .domain(d3.extent(that.base.data, function (d) { return that.base.translate.toDateFromJson(d.date); }));
    var y = d3.scale.linear().range([that.base.height, 0])
            .domain([0, d3.max(that.base.data, function (d) { return d.data.max * 1.1; })]);

    // Add the axis
    this.base.addAxisRotate(x, y, 10, 30);

    // Add the ticks
    this.base.addAxisTicks(x, y, this.base.data.length, 12);

    // clipping to start chart hidden and slide it in later
    this.base.svg.append('clipPath')
            .attr('id', 'trend_clip')
            .append('rect')
            .attr('width', that.base.width)
            .attr('height', that.base.height)
            .attr('transform', 'translate(' + that.base.margin.right + ',0)');

    this.base.svg.datum(this.base.data);

    var upperOuterArea = d3.svg.area()
        .interpolate('basis')
        .x(function (d) { return x(that.base.translate.toDateFromJson(d.date)) || 1; })
        .y0(function (d) { return y(d.data.perc_95); })
        .y1(function (d) { return y(d.data.quar_3); });

    var upperInnerArea = d3.svg.area()
        .interpolate('basis')
        .x(function (d) { return x(that.base.translate.toDateFromJson(d.date)) || 1; })
        .y0(function (d) { return y(d.data.quar_3); })
        .y1(function (d) { return y(d.data.quar_2); });

    var medianLine = d3.svg.line()
        .interpolate('basis')
        .x(function (d) { return x(that.base.translate.toDateFromJson(d.date)); })
        .y(function (d) { return y(d.data.quar_2); });

    var lowerInnerArea = d3.svg.area()
        .interpolate('basis')
        .x(function (d) { return x(that.base.translate.toDateFromJson(d.date)) || 1; })
        .y0(function (d) { return y(d.data.quar_2); })
        .y1(function (d) { return y(d.data.quar_1); });

    var lowerOuterArea = d3.svg.area()
        .interpolate('basis')
        .x(function (d) { return x(that.base.translate.toDateFromJson(d.date)) || 1; })
        .y0(function (d) { return y(d.data.quar_1); })
        .y1(function (d) { return y(d.data.perc_5); });

    this.base.svg.append('path')
        .attr('class', 'area upper outer')
        .attr('d', upperOuterArea)
        .attr('clip-path', 'url(#trend_clip)');

    this.base.svg.append('path')
        .attr('class', 'area lower outer')
        .attr('d', lowerOuterArea)
        .attr('clip-path', 'url(#trend_clip)');

    this.base.svg.append('path')
        .attr('class', 'area upper inner')
        .attr('d', upperInnerArea)
        .attr('clip-path', 'url(#trend_clip)');

    this.base.svg.append('path')
        .attr('class', 'area lower inner')
        .attr('d', lowerInnerArea)
        .attr('clip-path', 'url(#trend_clip)');

    this.base.svg.append('path')
        .attr('class', 'median-line')
        .attr('d', medianLine)
        .attr('clip-path', 'url(#trend_clip)');

    /*
    // Axis
    var xAxis = d3.svg.axis().scale(x).orient('bottom')
        .innerTickSize(-chartHeight).outerTickSize(0).tickPadding(10),
        yAxis = d3.svg.axis().scale(y).orient('left')
            .innerTickSize(-chartWidth).outerTickSize(0).tickPadding(10);

    var axes = svg.append('g')
        .attr('clip-path', 'url(#axes-clip)');

    axes.append('g')
        .attr('class', 'x axis')
        .attr('transform', 'translate(0,' + chartHeight + ')')
        .call(xAxis);

    axes.append('g')
        .attr('class', 'y axis')
        .call(yAxis)
        .append('text')
        .attr('transform', 'rotate(-90)')
        .attr('y', 6)
        .attr('dy', '.71em')
        .style('text-anchor', 'end')
        .text('Rendimiento');

    rectClip.transition()
        .duration(1000)
        .attr('width', chartWidth);
    
    // legend
    var legendWidth  = 200,
      legendHeight = 100;
      
    var legend = svg.append('g')
        .attr('class', 'legend')
        .attr('transform', 'translate(' + (chartWidth - legendWidth) + ', ' + (chartHeight - legendHeight) + ')');

    legend.append('rect')
        .attr('class', 'legend-bg')
        .attr('width', legendWidth)
        .attr('height', legendHeight);

    legend.append('rect')
        .attr('class', 'outer')
        .attr('width', 75)
        .attr('height', 20)
        .attr('x', 10)
        .attr('y', 10);

    legend.append('text')
        .attr('x', 115)
        .attr('y', 25)
        .text('5% - 95%');

    legend.append('rect')
        .attr('class', 'inner')
        .attr('width', 75)
        .attr('height', 20)
        .attr('x', 10)
        .attr('y', 40);

    legend.append('text')
        .attr('x', 115)
        .attr('y', 55)
        .text('25% - 75%');

    legend.append('path')
        .attr('class', 'median-line')
        .attr('d', 'M10,80L85,80');

    legend.append('text')
        .attr('x', 115)
        .attr('y', 85)
        .text('Media');*/

}