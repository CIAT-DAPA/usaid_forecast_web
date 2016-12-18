/**
 * This class contains all settings and some tools for the graphics
 * (string) container: Name of the div where the graphic will display
 */
function Base(container) {

    // Name of the div where the graphic will display
    this.container = container;

    // Width of container
    this.width = 0;
    // Height of container
    this.height = 0;

    // SVG 
    this.svg = null;

    // Animation settings
    this.animation = {
        duration: 1500,
        delay: 500
    }

    // Formats tho show the information or values in the graphics
    this.formats = {
        round : d3.format(",.0f"),
    };
}

/**
 * This method set the init values
 * (bool) relative: Indicates if the height if relative or not
 * (double) height: Height value of the graphic
 */
Base.prototype.init = function (relative, height) {
    var element = document.getElementById(this.container.replace('#',''));
    this.width = element.clientWidth;
    this.height = relative == true ? this.width * height : height;
    this.svg = d3.select(this.container).append("svg")
        .attr('width', this.width)
        .attr('height', this.height);
}

