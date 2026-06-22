<script setup lang="ts">
  import AvatarGroup from '@/@core/components/shared/AvatarGroup.vue'
  import { computed } from 'vue'
  import { useRouter } from 'vue-router'

  const router = useRouter()
  interface Supervisor {
    id?: number | string
    firstname?: string
    lastname?: string
    avatar_url?: string
    avatar?: string
    personal_image?: string
    [key: string]: any
  }

  interface TeamMember {
    id?: number | string
    name: string
    avatar?: string
    avatar_url?: string
    firstname?: string
    lastname?: string
    [key: string]: any
  }

  const props = withDefaults(defineProps<{
    id?: number | string | null
    title?: string
    companiesCount?: number
    locationsCount?: number
    shift?: string
    supervisor?: Supervisor | string
    teamMembers?: TeamMember[]
    inspectors?: TeamMember[]
  }>(), {
    id: null,
    title: '',
    companiesCount: 0,
    locationsCount: 0,
    shift: '',
    supervisor: () => ({}),
    teamMembers: () => [],
    inspectors: () => [],
  })

  const fallbackAvatar = 'https://placehold.co/400'

  // Format supervisor name
  const supervisorName = computed(() => {
    if (typeof props.supervisor === 'string') return props.supervisor
    if (!props.supervisor) return ''
    return `${props.supervisor.firstname} ${props.supervisor.lastname}`
  })

  // Format team members (supervisor + inspectors) for AvatarGroup
  const teamMembers = computed<TeamMember[]>(() => {
    // If teamMembers are directly provided, use them
    if (props.teamMembers && props.teamMembers.length) {
      return props.teamMembers
    }

    const members: TeamMember[] = []

    // Add supervisor if available
    const sup = props.supervisor
    if (sup && typeof sup !== 'string' && sup.id) {
      members.push({
        id: sup.id,
        name: supervisorName.value,
        avatar:
          sup.avatar_url ||
          sup.avatar ||
          sup.personal_image ||
          fallbackAvatar,
        avatar_url: sup.avatar_url || '',
      })
    }

    // Add inspectors if available
    if (props.inspectors && props.inspectors.length) {
      props.inspectors.forEach((inspector: TeamMember) => {
        members.push({
          id: inspector.id,
          name: `${inspector.firstname} ${inspector.lastname}`,
          avatar:
            inspector.avatar_url ||
            inspector.avatar ||
            inspector.personal_image ||
            fallbackAvatar,
          avatar_url: inspector.avatar_url || '',
        })
      })
    }

    return members
  })

  // Map teamMembers to the format expected by AvatarGroup
  const avatars = computed(() =>
    teamMembers.value.map((member: TeamMember) => ({
      name: member.name,
      avatar: member.avatar,
    }))
  )

  // Navigate to team management page
  const navigateToTeamManagement = () => {
    if (props.id) {
      router.push({ name: 'team-management', params: { id: props.id } })
    }
  }
</script>

<template>
  <VCard
    class="rounded-lg"
    :class="{ 'opacity-75': !id }"
    style="box-shadow: 0px 4px 44px 4px rgba(0, 0, 0, 0.04)"
  >
    <v-card-text class="pa-3 pa-sm-4">
      <div class="d-flex justify-space-between align-center ga-1">
        <div>
          <h2 style="max-width: 200px; display: inline-block">
            <RouterLink
              class="text-h6 text-black text-truncate"
              :to="{
                name: 'team-management',
                params: { id },
              }"
            >
              {{ title }}
            </RouterLink>
          </h2>
        </div>
        <div class="d-flex align-center ga-2">
          <span>
            <svg
              fill="none"
              height="1.4rem"
              viewBox="0 0 17 16"
              width="1.4rem"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M9.83344 5.49998C9.40895 5.49998 9.03409 5.52147 8.71559 5.59144C8.39143 5.66265 8.09458 5.79073 7.85938 6.02593C7.62418 6.26113 7.49611 6.55798 7.42489 6.88214C7.35492 7.20064 7.33344 7.5755 7.33344 7.99998L7.33344 13.8333H10.4998V12.6667C10.4998 12.2985 10.7983 12 11.1665 12C11.5347 12 11.8331 12.2985 11.8331 12.6667V13.8333H15.0001L15.0001 7.99998C15.0001 7.5755 14.9786 7.20064 14.9086 6.88214C14.8374 6.55798 14.7094 6.26113 14.4742 6.02593C14.239 5.79073 13.9421 5.66265 13.6179 5.59144C13.2994 5.52147 12.9246 5.49998 12.5001 5.49998H9.83344Z"
                fill="#667178"
                opacity="0.4"
              />
              <path
                d="M1.3335 14.5C1.3335 14.1318 1.63197 13.8333 2.00016 13.8333L2.00016 9.83331H3.8332C4.10934 9.83331 4.3332 9.60946 4.3332 9.33331C4.3332 9.05717 4.10934 8.83331 3.8332 8.83331H2.00016V7.16665L3.8332 7.16665C4.10934 7.16665 4.3332 6.94279 4.3332 6.66665C4.3332 6.3905 4.10934 6.16665 3.8332 6.16665L2.00016 6.16665L2.00016 4.49998H3.8332C4.10934 4.49998 4.3332 4.27612 4.3332 3.99998C4.3332 3.72384 4.10934 3.49998 3.8332 3.49998L2.00016 3.49998V3.33331C2.00016 2.90882 2.02165 2.53397 2.09162 2.21547C2.16283 1.89131 2.29091 1.59446 2.52611 1.35926C2.76131 1.12406 3.05815 0.995983 3.38232 0.92477C3.70082 0.854802 4.07568 0.833313 4.50016 0.833313L8.50016 0.833313C8.92465 0.833313 9.29951 0.854802 9.61801 0.92477C9.94217 0.995983 10.239 1.12406 10.4742 1.35926C10.7094 1.59446 10.8375 1.89131 10.9087 2.21547C10.9787 2.53397 11.0002 2.90882 11.0002 3.33331V4.09998C11.0002 4.28854 11.0002 4.38282 10.9416 4.4414C10.883 4.49998 10.7887 4.49998 10.6002 4.49998H9.8335C9.38714 4.49998 8.92635 4.52131 8.50109 4.61473C8.05909 4.71183 7.56497 4.90619 7.15234 5.31882C6.7397 5.73145 6.54534 6.22557 6.44824 6.66757C6.35482 7.09283 6.3335 7.55362 6.3335 7.99998L6.3335 13.8333H10.5002V12.6666C10.5002 12.2985 10.7986 12 11.1668 12C11.535 12 11.8335 12.2985 11.8335 12.6666V13.8333L15.0002 13.8333C15.3684 13.8333 15.6668 14.1318 15.6668 14.5C15.6668 14.8682 15.3684 15.1666 15.0002 15.1666L2.00016 15.1666C1.63197 15.1666 1.3335 14.8682 1.3335 14.5Z"
                fill="#667178"
              />
              <path
                d="M10.0002 7.99998C10.0002 7.72384 10.224 7.49998 10.5002 7.49998H11.8335C12.1096 7.49998 12.3335 7.72384 12.3335 7.99998C12.3335 8.27612 12.1096 8.49998 11.8335 8.49998H10.5002C10.224 8.49998 10.0002 8.27612 10.0002 7.99998Z"
                fill="#667178"
              />
              <path
                d="M10.0002 9.99998C10.0002 9.72384 10.224 9.49998 10.5002 9.49998H11.8335C12.1096 9.49998 12.3335 9.72384 12.3335 9.99998C12.3335 10.2761 12.1096 10.5 11.8335 10.5H10.5002C10.224 10.5 10.0002 10.2761 10.0002 9.99998Z"
                fill="#667178"
              />
            </svg>
          </span>
          <span style="color: #75797c">
            <span>{{ locationsCount || companiesCount }}</span>
            <span> موقع</span>
          </span>
        </div>
      </div>

      <VDivider class="my-3" />

      <!-- Team Members Section -->
      <div class="team-members">
        <!-- Supervisor Section -->
        <div class="d-flex align-center mb-2">
          <span class="text-subtitle-2 font-weight-medium me-2">المشرف:</span>
          <span class="text-body-2">{{ supervisorName }}</span>
        </div>

        <!-- Shift Section if available -->
        <div v-if="shift" class="d-flex align-center mb-2">
          <span class="text-subtitle-2 font-weight-medium me-2">الفترة:</span>
          <span class="text-body-2">{{ shift }}</span>
        </div>

        <!-- Inspectors Section -->
        <div class="d-flex align-center justify-space-between">
          <div
            v-if="teamMembers && teamMembers.length"
            class="d-flex flex-column mt-2"
          >
            <div class="d-flex flex-wrap mt-1 position-relative">
              <div
                v-for="(member, index) in teamMembers.slice(0, 5)"
                :key="index"
                class="position-relative"
                style="margin-left: -10px"
              >
                <v-avatar class="border-2 border-white" size="30">
                  <v-img
                    :alt="member.name"
                    cover
                    height="30"
                    :src="member.avatar_url || 'https://placehold.co/600x400'"
                    width="30"
                  >
                    <template #placeholder>
                      <div
                        class="d-flex align-center justify-center fill-height"
                      >
                        <v-icon
                          color="grey-lighten-1"
                          icon="mdi-account"
                          size="20"
                        />
                      </div>
                    </template>
                  </v-img>
                </v-avatar>
              </div>
              <span
                v-if="teamMembers.length > 5"
                class="text-body-2 font-weight-medium ms-2 d-flex align-center ms-4"
              >
                +{{ teamMembers.length - 5 }}
              </span>
            </div>
          </div>
          <v-btn
            class="btn-card-link px-3 text-primary rounded-xl"
            elevation="0"
            size="small"
            :to="{
              name: 'team-management',
              params: { id },
            }"
          >
            <span class="font-weight-bold">عرض</span>
            <svg
              fill="none"
              height="1.1rem"
              style="rotate: 180deg"
              viewBox="0 0 16 16"
              width="1.1rem"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M6.38 12.2867C6.28667 12.2867 6.19333 12.2533 6.12 12.18C5.97333 12.0333 5.97333 11.7867 6.12 11.64L9.75333 8.00668L6.12 4.37334C5.97333 4.22668 5.97333 3.98001 6.12 3.83334C6.26667 3.68668 6.51333 3.68668 6.66 3.83334L10.6133 7.78668C10.76 7.93334 10.76 8.18001 10.6133 8.32668L6.66 12.28C6.56667 12.2533 6.47333 12.2867 6.38 12.2867Z"
                fill="#667178"
              />
            </svg>
          </v-btn>
        </div>
      </div>
    </v-card-text>
  </VCard>
</template>
