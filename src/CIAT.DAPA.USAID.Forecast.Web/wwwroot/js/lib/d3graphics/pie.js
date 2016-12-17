/**
 * This class draws a pie
 * (Base) base: Configuration to render the graphic
 */
function Pie(base) {
    this.base = base;
}

Pie.prototype.drawChartCenter = function (pie, radius) {
    var centerContainer = pie.append('g')
      .attr('class', 'pieChart--center');

    centerContainer.append('circle')
      .attr('class', 'pieChart--center--outerCircle')
      .attr('r', 0)
      .attr('filter', 'url(#pieChartDropShadow)')
      .transition()
      .duration(D3Graphics.Pie.vars.DURATION)
      .delay(D3Graphics.Pie.vars.DELAY)
      .attr('r', radius - 50);

    centerContainer.append('circle')
      .attr('id', 'pieChart-clippy')
      .attr('class', 'pieChart--center--innerCircle')
      .attr('r', 0)
      .transition()
      .delay(D3Graphics.Pie.vars.DELAY)
      .duration(D3Graphics.Pie.vars.DURATION)
      .attr('r', radius - 55)
      .attr('fill', '#fff');
}

var D3Graphics = D3Graphics || {};

D3Graphics.Pie = D3Graphics.Pie || {};

D3Graphics.Pie.vars = {
    DURATION: 1500,
    DELAY: 500,
    container: 'pieChart'
}

D3Graphics.Pie.tools = {
    drawChartCenter: function (pie, radius) {
        var centerContainer = pie.append('g')
          .attr('class', 'pieChart--center');

        centerContainer.append('circle')
          .attr('class', 'pieChart--center--outerCircle')
          .attr('r', 0)
          .attr('filter', 'url(#pieChartDropShadow)')
          .transition()
          .duration(D3Graphics.Pie.vars.DURATION)
          .delay(D3Graphics.Pie.vars.DELAY)
          .attr('r', radius - 50);

        centerContainer.append('circle')
          .attr('id', 'pieChart-clippy')
          .attr('class', 'pieChart--center--innerCircle')
          .attr('r', 0)
          .transition()
          .delay(D3Graphics.Pie.vars.DELAY)
          .duration(D3Graphics.Pie.vars.DURATION)
          .attr('r', radius - 55)
          .attr('fill', '#fff');
    },
    drawDetailedInformation: function (data, element, width, detailedInfo, height) {
        var bBox = element.getBBox(),
          infoWidth = width * 0.3,
          anchor,
          infoContainer,
          position;
        console.log(bBox);
        console.log(element);

        var y = bBox.height + bBox.y + 50;
        //var y= height - (bBox.height / 2) + bBox.y  ;

        if ((bBox.x + bBox.width / 2) > 0) {
            infoContainer = detailedInfo.append('g')
              .attr('width', infoWidth)
              .attr('transform', 'translate(' + (width - infoWidth) + ',' + y + ')');
            anchor = 'end';
            position = 'right';
        } else {
            infoContainer = detailedInfo.append('g')
              .attr('width', infoWidth)
              .attr('transform', 'translate(' + 0 + ',' + y + ')');
            anchor = 'start';
            position = 'left';
        }

        console.log(infoContainer.attr('transform'));


        infoContainer.data([data.value * 100])
          .append('text')
          .text('0 %')
          .attr('class', 'pieChart--detail--percentage')
          .attr('x', (position === 'left' ? 0 : infoWidth))
          .attr('y', -10)
          .attr('text-anchor', anchor)
          .transition()
          .duration(D3Graphics.Pie.vars.DURATION)
          .tween('text', function (d) {
              var i = d3.interpolateRound(+this.textContent.replace(/\s%/ig, ''), d);
              return function (t) { this.textContent = i(t) + ' %'; };
          });

        infoContainer.append('line')
          .attr('class', 'pieChart--detail--divider')
          .attr('x1', 0)
          .attr('x2', 0)
          .attr('y1', 0)
          .attr('y2', 0)
          .transition()
          .duration(D3Graphics.Pie.vars.DURATION)
          .attr('x2', infoWidth);

        infoContainer.data([data.description])
          .append('foreignObject')
          .attr('width', infoWidth)
          .attr('height', 100)
          .append('xhtml:body')
          .attr('class', 'pieChart--detail--textContainer pieChart--detail__' + position)
          .html(data.description);

    }


}

D3Graphics.Pie.render = function (data) {

    // TODO code duplication check how you can avoid that
    var containerEl = document.getElementById(D3Graphics.Pie.vars.container),
      width = containerEl.clientWidth,
      height = width * 0.4,
      radius = Math.min(width, height) / 2,
      container = d3.select(containerEl),
      svg = container.select('svg')
        .attr('width', width)
        .attr('height', height);
    var pie = svg.append('g')
      .attr(
      'transform',
      'translate(' + width / 2 + ',' + height / 2 + ')'
      );

    var detailedInfo = svg.append('g')
      .attr('class', 'pieChart--detailedInformation');

    var twoPi = 2 * Math.PI;
    var pieData = d3.layout.pie()
      .value(function (d) { return d.value; });

    var arc = d3.svg.arc()
      .outerRadius(radius - 20)
      .innerRadius(0);

    var pieChartPieces = pie.datum(data)
      .selectAll('path')
      .data(pieData)
      .enter()
      .append('path')
      .attr('class', function (d) {
          return 'pieChart__' + d.data.color;
      })
      .attr('filter', 'url(#pieChartInsetShadow)')
      .attr('d', arc)
      .each(function () {
          this._current = { startAngle: 0, endAngle: 0 };
      })
      .transition()
      .duration(D3Graphics.Pie.vars.DURATION)
      .attrTween('d', function (d) {
          var interpolate = d3.interpolate(this._current, d);
          this._current = interpolate(0);

          return function (t) {
              return arc(interpolate(t));
          };
      })
      .each('end', function handleAnimationEnd(d) {
          D3Graphics.Pie.tools.drawDetailedInformation(d.data, this, width, detailedInfo, height);
      });

    D3Graphics.Pie.tools.drawChartCenter(pie, radius);
}