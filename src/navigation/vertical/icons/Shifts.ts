import { h } from 'vue'

const ShiftsIcon = {
  name: 'ShiftsIcon',
  render () {
    return h(
      'svg',
      {
        xmlns: 'http://www.w3.org/2000/svg',
        width: '24',
        height: '24',
        viewBox: '0 0 24 24',
        fill: 'none',
        stroke: 'currentColor',
        'stroke-width': '2',
        'stroke-linecap': 'round',
        'stroke-linejoin': 'round',
      },
      [
        h('circle', {
          cx: '17',
          cy: '6',
          r: '2',
          fill: 'white',
          stroke: 'none',
        }),
        h('path', { d: 'M19 17h-4l-4-10H7' }),
        h('path', { d: 'M9 18a2 2 0 1 0 0-4 2 2 0 0 0 0 4z' }),
        h('path', { d: 'M17 22a2 2 0 1 0 0-4 2 2 0 0 0 0 4z' }),
        h('path', { d: 'M11 7h4l2 10' }),
      ],
    )
  },
}

export default ShiftsIcon
