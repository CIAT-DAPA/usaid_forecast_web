/**
 * This class draws a pie
 * (Base) base: Configuration to render the graphic
 */
function Pie(base) {
    this.base = base;
    this.centerContainer = null;
    this.radius = 50;
    this.twoPi = 2 * Math.PI;
}

/**
 * Method tha draw a white circle in the center of the graphic, so this way it gave an apparence of radial
 * (object) pie: Graphic in the which should add the new graphic
*/
Pie.prototype.drawChartCenter = function (pie, content) {
    var that = this;
    var centerContainer = pie.append('g')
      .attr('class', 'pie_center');

    centerContainer.append('circle')
      .attr('class', 'pie_center_outer_circle')
      .attr('r', 0)
      .attr('filter', 'url(#pieChartDropShadow)')
      .transition()
      .duration(this.base.animation.duration)
      .delay(this.base.animation.delay)
      .attr('r', this.radius * 0.4);

    centerContainer.append('circle')
      .attr('id', 'pie_clippy')
      .attr('class', 'pie_center_inner_circle')
      .attr('r', 0)
      .transition()
      .delay(this.base.animation.delay)
      .duration(this.base.animation.duration)
      .attr('r', this.radius * 0.5)
      .attr('fill', '#fff');

    centerContainer.append("text")
      /*.attr('dx', function (d) {
          var l = content.toString().length;
          return l >= 3 ? -17 : (l == 2 ? -10 : -5);
      })*/
      .attr('class', 'pie_center_text_high')
      .style("text-anchor", "middle")
      .text(function (d) { return content; });

    centerContainer.append("text")
      .attr('dx', function (d) { return -16; })
      .attr('dy', function (d) { return 10; })
      .attr('class', 'pie_center_text_small')
      .text(function (d) { return 'Normal'; });
}

/*
 * Method that render the graphic in a container
*/
Pie.prototype.render = function () {

    this.base.init(true, 1);
    this.radius = Math.min(this.base.width, this.base.height) / 2;
    var pie = this.base.svg.append('g')
                  .attr('transform', 'translate(' + this.base.width / 2 + ',' + this.base.height / 2 + ')');
    var pieData = d3.layout.pie()
                    .value(function (d) { return d.value; });
    var arc = d3.svg.arc()
                .outerRadius(this.radius * 0.8)
                .innerRadius(0);
    var pieChartPieces = pie.datum(this.base.data.percentages)
                          .selectAll('path')
                          .data(pieData)
                          .enter()
                          .append('path')
                          .attr('class', function (d) {
                              return 'pie_' + d.data.type;
                          })
                          .attr('d', arc)
                          .each(function () {
                              this._current = { startAngle: 0, endAngle: 0 };
                          })
                          .transition()
                          .duration(this.base.animation.duration)
                          .attrTween('d', function (d) {
                              var interpolate = d3.interpolate(this._current, d);
                              this._current = interpolate(0);
                              return function (t) {
                                  return arc(interpolate(t));
                              };
                          })
                          .each('end', function handleAnimationEnd(d) {
                          });

    // Draw the center and add the text
    this.drawChartCenter(pie, this.base.data.center);
    // Draw the legend
    this.base.addLegend('bottom', this.base.data.percentages.map(function (item) {
        return { title: item.label, value: (item.value * 100) + '%', class: 'pie_' + item.type };
    }));
}
