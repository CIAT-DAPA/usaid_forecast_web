/**
 * This class draws a bars graphic
 * (Base) base: Configuration to render the graphic
 */
function Bars(base) {
    this.base = base;

    this.space = 0.1;
}

/*
 * Method that render the graphic in a container
*/
Bars.prototype.render = function () {
    var that = this;
    this.base.init(true, 0.5);

    //create svg for histogram.
    this.base.svg.append("g")
                .attr("transform", "translate(0,0)");

    // create function for x-axis mapping.
    var x = d3.scale.ordinal().rangeRoundBands([this.base.margin.right, this.base.width_full - this.base.margin.left], this.space)
                .domain(this.base.data.map(function (d) { return d.month_name; }));

    // Create function for y-axis map.
    var y = d3.scale.linear().range([this.base.height, this.base.getMarginVertical()])
                .domain([0, d3.max(this.base.data, function (d) { return d.value; }) * 1.1]);

    // Add the axis
    this.base.addAxis(x, y, 8);
   
    // Create bars for histogram to contain rectangles and freq labels.
    var bars = this.base.svg.selectAll(".bar")
                    .data(this.base.data)
                    .enter()
                    .append("g")
                    .attr("class", "bar");

    //create the rectangles.
    bars.append("rect")
        .attr("x", function (d) { return x(d.month_name); })
        .attr("y", function (d) { return y(d.value); })
        .attr("width", x.rangeBand())
        .attr("height", function (d) { return (that.base.height) - y(d.value); })
        .attr('class', 'bar_color');

    //Create the frequency labels above the rectangles.
    bars.append("text").text(function (d) { return that.base.formats.float(d.value) })
        .attr("x", function (d) { return x(d.month_name) + x.rangeBand() / 2; })
        .attr("y", function (d) { return y(d.value) - 5; })
        .attr("text-anchor", "normaldle");
}