/**
 * This class contains all settings and some tools for the graphics
 * (string) container: Name of the div where the graphic will display
 */
function Base(container) {

    // Name of the div where the graphic will display
    this.container = container;

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

    // 
    this.space = 0.1;
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
 * Method that get full margin veritcal of the graphic
*/
Base.prototype.getMarginVertical = function(){
    return (this.margin.top + this.margin.bottom);
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

/**
 * This method set the init values
 * (bool) relative: Indicates if the height if relative or not
 * (double) height: Height value of the graphic
 */
Base.prototype.init = function (relative, height) {
    var element = document.getElementById(this.container.replace('#', ''));
    this.width_full = element.clientWidth;
    this.height_full = relative == true ? this.width_full * height : height;
    this.width = this.width_full - (this.margin.left + this.margin.right);
    this.height = this.height_full - (this.margin.top + this.margin.bottom);
    this.svg = d3.select(this.container).append("svg")
        .attr('width', this.width_full)
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
Base.prototype.addAxis = function (x,y,ticks) {
    // Add x-axis to the histogram svg.
    this.svg.append("g").attr("class", "x axis")
        .attr("transform", "translate(0," + (this.height) + ")")
        .call(this.getXAxis(x));

    // Add y-axis to the histogram svg.
    this.svg.append("g")
        .attr("class", "y axis")
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
        .attr('class', 'lineChart--xAxisTicks')
        .attr('transform', 'translate(' + this.margin.right / 2 + ',' + this.height + ')')
        .call(this.getTicks(x,x_ticks,-this.height));
    
    this.svg.append('g')
        .attr('class', 'lineChart--yAxisTicks')
        .call(this.getTicks(y, y_ticks, this.width).orient('right'));
}
