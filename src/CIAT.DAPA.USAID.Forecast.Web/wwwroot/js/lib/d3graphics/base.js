/**
 * This class contains all settings and some tools for the graphics
 * (string) container: Name of the div where the graphic will display
 * (object) data: Set of information
 */
function Base(container, data) {

    // Name of the div where the graphic will display
    this.container = container;
    // Data of the graphic
    this.data = data;

    // Width of container    
    this.width_full = 0;
    // Width whit margin
    this.width = 0;
    // Height of container    
    this.height_full = 0;
    // Height whit margin
    this.height = 0;
    // Margin
    this.margin = {
        top: 0,
        right: 0,
        left: 0,
        bottom: 0
    };

    // SVG 
    this.svg = null;
    this.g = null;

    // Animation settings
    this.animation = {
        duration: 1500,
        delay: 500
    }

    // Formats tho show the information or values in the graphics
    this.formats = {
        round: d3.format(",.0f"),
        date: d3.time.format('%Y-%m-%d').parse
    };

}

/*
 * Method that set the same size for the margin of the graphic
 * (int) size: Pixel of margin
*/
Base.prototype.setMargin = function (size) {
    this.margin.top = size;
    this.margin.right = size;
    this.margin.left = size;
    this.margin.bottom = size;
}

/*
 * Method that set the same size for the margin of the graphic
 * (int) top: Margin top
 * (int) right: Margin right
 * (int) left: Margin left
 * (int) bottom: Margin bottom
*/
Base.prototype.setMargin = function (top, right, left, bottom) {
    this.margin.top = top;
    this.margin.right = right;
    this.margin.left = left;
    this.margin.bottom = bottom;

}

/*
 * Method that get full margin veritcal of the graphic
*/
Base.prototype.getMarginVertical = function () {
    return (this.margin.top + this.margin.bottom);
}

/**
 * This method set the init values
 * (bool) relative: Indicates if the height if relative or not
 * (double) height: Height value of the graphic
 */
Base.prototype.init = function (relative, height) {
    this.svg = d3.select(this.container).append("svg");
    this.g = this.svg.append('g');
    this.update(relative, height);
}

/*
 * Method that update the graphic's dimensions
 * (bool) relative: Indicates if the height if relative or not
 * (double) height: Height value of the graphic
*/
Base.prototype.update = function (relative, height) {
    var element = document.getElementById(this.container.replace('#', ''));
    this.width_full = element.clientWidth;
    this.height_full = relative == true ? this.width_full * height : height;
    this.width = this.width_full - (this.margin.left + this.margin.right);
    this.height = this.height_full - (this.margin.top + this.margin.bottom);
    this.svg.attr('width', this.width_full)
            .attr('height', this.height_full);
}

/*
 * Method that create a scale for y axis
 * (function) y: Function to scale the y axis
 * (int) ticks: Count of splitters
*/
Base.prototype.getYAxis = function (y, ticks) {
    return d3.svg.axis().scale(y).ticks(ticks).orient("left");
}

/*
 * Method that create a scale for x axis
 * (function) x: Function to scale the x axis
*/
Base.prototype.getXAxis = function (x) {
    return d3.svg.axis().scale(x).orient("bottom");
}

/*
 * Method that create a function for the ticks
 * (function) xy: X or Y function to create interpolation
 * (int) ticks: Count of ticks
 * (double) size: Size of the ticks
*/
Base.prototype.getTicks = function (xy, ticks, size) {
    return d3.svg.axis().scale(xy)
            .ticks(ticks)
            .tickSize(size)
            .tickFormat('');
}

/*
 * Method that add the axis x and y in the graphic
 * (function) x: Function to interpolate the x values
 * (function) y: Function to interpolate the y values
 * (int) ticks: Count of ticks
*/
Base.prototype.addAxis = function (x, y, ticks) {
    // Add x-axis to the histogram svg.
    this.svg.append("g").attr("class", "x_axis")
        .attr("transform", 'translate(0,' + this.height + ')')
        .call(this.getXAxis(x));

    // Add y-axis to the histogram svg.
    this.svg.append("g")
        .attr("class", "y_axis")
        .attr("transform", "translate(35,0)")
        .call(this.getYAxis(y, ticks));
}

/*
 * Method that add lines ticks in the graphic
 * (function) x: Function to interpolate the x axis
 * (function) y: Function to interpolate the y axis
 * (int) x_ticks: Count of the ticks in x axis
 * (int) y_ticks: Count of the ticks in y axis
*/
Base.prototype.addAxisTicks = function (x, y, x_ticks, y_ticks) {
    this.svg.append('g')
        .attr('class', 'x_axis_ticks')
        .attr('transform', 'translate(' + this.margin.right / 2 + ',' + this.height + ')')
        .call(this.getTicks(x, x_ticks, -this.height));

    this.svg.append('g')
        .attr('class', 'y_axis_ticks')
        .attr('transform', 'translate(' + this.margin.right + ',0)')
        .call(this.getTicks(y, y_ticks, this.width).orient('right'));
}

/*
 * Method that add legend to the graphic
 * (string) location: Place where you want to put the legend (bottom, right)
 * (object) content: Information that should the legend has ({title,value,class})
*/
Base.prototype.addLegend = function (location, content) {
    var that = this;

    // Create function for y-axis map.
    var y_legend = d3.scale.ordinal()
                        .domain(content.map(function (item) { return item.title; }))
                        .rangePoints([0, 15 * content.length]);
    var legend = null;

    // Select the location of the legend
    if (location === 'bottom') {
        this.svg.attr('height', this.height_full + (20 * content.length));
        legend = this.svg.append('g');
        legend.attr('transform', 'translate(0,' + this.height +')');
    }
    else {
        this.svg.attr('width', this.width_full * 1.3);
        legend = this.svg.append('g');
        legend.attr('transform', 'translate(' + this.width + ',0)');
    }

    // Add rect with color of every item
    legend.selectAll(".legend")
          .data(content)
          .enter()
          .append("rect")
          .attr("x", '0')
          .attr("y", function (d) { return y_legend(d.title) - 15; })
          .attr("width", '20')
          .attr("height", '20')
          .attr('class', function (d) { return d.class; });
    // Add text with titles of the legend
    legend.selectAll(".legend")
          .data(content)
          .enter()
          .append('text')
          .attr("x", '22')
          .attr("y", function (d) { return y_legend(d.title); })
          .text(function (d) { return d.value + ' - ' + d.title; });
}