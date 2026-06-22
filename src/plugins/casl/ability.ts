import { Ability } from '@casl/ability'

export const initialAbility = [
  {
    action: 'read',
    subject: 'Auth',
  },
]

const stringifiedUserAbilities = localStorage.getItem('userAbilities')
const existingAbility = stringifiedUserAbilities ? JSON.parse(stringifiedUserAbilities) : null
export default new Ability(existingAbility || initialAbility)
