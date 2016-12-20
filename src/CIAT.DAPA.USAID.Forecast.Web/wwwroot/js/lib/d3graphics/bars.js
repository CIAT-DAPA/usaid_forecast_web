/**
 * This class draws a bars graphic
 * (Base) base: Configuration to render the graphic
 */
function Bars(base) {
    this.base = base;

    this.space = 0.1;
}

// function to handle histogram.
Bars.prototype.render = function () {
    var that = this;
    console.log(this);
    this.base.init(true, 0.5);

    //create svg for histogram.
    this.base.svg.append("g")
                .attr("transform", "translate(0,0)");

    // create function for x-axis mapping.
    var x = d3.scale.ordinal().rangeRoundBands([this.base.margin.right, this.base.width], this.space)
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
    //.on("mouseover", mouseover)// mouseover is defined bebajo.
    //.on("mouseout", mouseout);// mouseout is defined bebajo.

    //Create the frequency labels above the rectangles.
    bars.append("text").text(function (d) { return that.base.formats.round(d.value) })
        .attr("x", function (d) { return x(d.month_name) + x.rangeBand() / 2; })
        .attr("y", function (d) { return y(d.value) - 5; })
        .attr("text-anchor", "normaldle");
    /*
    function mouseover(d) {  // utility function to be called on mouseover.
        // filter for selected state.
        var st = fData.filter(function (s) { return s.State == d[0]; })[0],
            nD = d3.keys(st.freq).map(function (s) { return { type: s, freq: st.freq[s] }; });

        // call update functions of pie-chart and legend.    
        pC.update(nD);
        leg.update(nD);
        $("#month").text(d[0]);
    }

    function mouseout(d) {    // utility function to be called on mouseout.
        // reset the pie-chart and legend.    
        //pC.update(tF);
        //leg.update(tF);
    }*/
}