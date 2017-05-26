/**
 * This class draws a trend graphic
 * (Base) base: Configuration to render the graphic
 */
function Trend(base) {
    this.base = base;
}

Trend.prototype.dateParse = function (date) {
    return this.base.translate.toDateFromJson(date);
}

/*
 * Method
*/
Trend.prototype.render = function () {    
    var that = this;
    that.base.init(true, 0.5);

    // Interpolation function
    var x = d3.time.scale().range([that.base.margin.right, that.base.width_full - that.base.margin.left])
            .domain(d3.extent(that.base.data, function (d) { return that.dateParse(d.date); }));
    var y = d3.scale.linear().range([that.base.height, 0])
            .domain([d3.min(that.base.data, function (d) { return d.data.min; }), d3.max(that.base.data, function (d) { return d.data.max; }) * 1.05]);

    // Add the axis
    this.base.addAxisDate(x, y, 10, 45);

    // Add the ticks
    //this.base.addAxisTicks(x, y, this.base.data.length, 12);
    this.base.addAxisTicks(x, y, 12, 12);

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
        .x(function (d) { return x(that.dateParse(d.date)) || 1; })
        .y0(function (d) { return y(d.data.perc_95); })
        .y1(function (d) { return y(d.data.quar_3); });

    var upperInnerArea = d3.svg.area()
        .interpolate('basis')
        .x(function (d) { return x(that.dateParse(d.date)) || 1; })
        .y0(function (d) { return y(d.data.quar_3); })
        .y1(function (d) { return y(d.data.quar_2); });

    var medianLine = d3.svg.line()
        .interpolate('basis')
        .x(function (d) { return x(that.dateParse(d.date)); })
        .y(function (d) { return y(d.data.quar_2); });

    var lowerInnerArea = d3.svg.area()
        .interpolate('basis')
        .x(function (d) { return x(that.dateParse(d.date)) || 1; })
        .y0(function (d) { return y(d.data.quar_2); })
        .y1(function (d) { return y(d.data.quar_1); });

    var lowerOuterArea = d3.svg.area()
        .interpolate('basis')
        .x(function (d) { return x(that.dateParse(d.date)) || 1; })
        .y0(function (d) { return y(d.data.quar_1); })
        .y1(function (d) { return y(d.data.perc_5); });

    this.base.svg.append('path')
        //.attr('class', 'area upper outer')
        .attr('class', 'trend_yield_area_outer')
        .attr('d', upperOuterArea)
        .attr('clip-path', 'url(#trend_clip)');

    this.base.svg.append('path')
        //.attr('class', 'area lower outer')
        .attr('class', 'trend_yield_area_outer')
        .attr('d', lowerOuterArea)
        .attr('clip-path', 'url(#trend_clip)');

    this.base.svg.append('path')
        //.attr('class', 'area upper inner')
        .attr('class', 'trend_yield_area_inner')
        .attr('d', upperInnerArea)
        .attr('clip-path', 'url(#trend_clip)');

    this.base.svg.append('path')
        //.attr('class', 'area lower inner')
        .attr('class', 'trend_yield_area_inner')
        .attr('d', lowerInnerArea)
        .attr('clip-path', 'url(#trend_clip)');

    this.base.svg.append('path')
        .attr('class', 'trend_yield_median_line')
        .attr('d', medianLine)
        .attr('clip-path', 'url(#trend_clip)');

    
    
    // legend
    var legendWidth  = 200,
      legendHeight = 100;
      
    var legend = this.base.svg.append('g')
        .attr('class', 'legend')
        .attr('transform', 'translate(' + (that.base.width_full - legendWidth) + ', ' + ( 0) + ')');

    legend.append('rect')
        .attr('class', 'trend_legend_bg')
        .attr('width', legendWidth)
        .attr('height', legendHeight);

    legend.append('rect')
        .attr('class', 'trend_yield_legend_outer')
        .attr('width', 75)
        .attr('height', 20)
        .attr('x', 10)
        .attr('y', 10);

    legend.append('text')
        .attr('x', 92)
        .attr('y', 25)
        .text('Perc. 5 y 95');

    legend.append('rect')
        .attr('class', 'trend_yield_legend_inner')
        .attr('width', 75)
        .attr('height', 20)
        .attr('x', 10)
        .attr('y', 40);

    legend.append('text')
        .attr('x', 92)
        .attr('y', 55)
        .text('Int. conf. (95%)');

    legend.append('path')
        .attr('class', 'trend_yield_median_line')
        .attr('d', 'M10,80L85,80');

    legend.append('text')
        .attr('x', 92)
        .attr('y', 85)
        .text('Promedio');

}