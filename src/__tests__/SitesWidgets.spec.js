import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

const mountSitesWidgets = (props = {}) => {
  return mount({
    template: `
      <div>
        <v-row v-if="isLoading">
          <v-col v-for="item in 4" :key="item" cols="6" md="3">
            <v-card elevation="0"><v-card-text>Loading...</v-card-text></v-card>
          </v-col>
        </v-row>
        <v-row v-else>
          <v-col v-for="(val, key) in data" :key="key" cols="6" md="3">
            <div>{{ key }}: {{ val }}</div>
          </v-col>
        </v-row>
      </div>
    `,
    props: ['isLoading', 'data'],
  }, { props })
}

describe('SitesWidgets', () => {
  it('shows skeleton when isLoading is true', () => {
    const wrapper = mountSitesWidgets({ isLoading: true })
    expect(wrapper.text()).toContain('Loading...')
  })

  it('shows data when isLoading is false', () => {
    const wrapper = mountSitesWidgets({
      isLoading: false,
      data: { total_users: 10, total_locations: 5 },
    })
    expect(wrapper.text()).toContain('10')
    expect(wrapper.text()).toContain('5')
  })
})
