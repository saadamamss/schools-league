<template>
  <v-container class="profile-container" fluid>
    <v-skeleton-loader v-if="!user" type="card-avatar" />
    <v-card v-else>
      <v-card-text>
        <v-row>
          <!-- Profile Column -->
          <v-col class="text-center" cols="12" md="4">
            <v-avatar class="mb-4" color="primary" size="200">
              <span class="text-h2">
                {{ user?.firstName?.[0] }}
              </span>
            </v-avatar>

            <h2 class="text-h5">{{ user.fullName }}</h2>
            <div class="d-flex justify-center mt-2 ga-3">
              <v-chip class="mr-2" color="primary">
                {{ user.userType.name.ar }}
              </v-chip>
              <v-chip :color="user.isActive ? 'primary' : 'error'">
                {{ user.isActive ? "نشط" : "غير نشط" }}
              </v-chip>
            </div>

            <v-card class="mt-6" variant="outlined">
              <v-card-text>
                <div class="text-subtitle-1 mb-2">معلومات الاتصال</div>
                <v-divider class="mb-3" />

                <v-list density="compact">
                  <v-list-item>
                    <template #prepend>
                      <v-icon>mdi-email</v-icon>
                    </template>
                    <v-list-item-title>{{ user.email }}</v-list-item-title>
                    <template #append>
                      <v-icon
                        v-if="user.email_verified"
                        color="success"
                      >mdi-check-decagram</v-icon>
                    </template>
                  </v-list-item>

                  <v-list-item>
                    <template #prepend>
                      <v-icon>mdi-phone</v-icon>
                    </template>
                    <v-list-item-title>{{ user.phone }}</v-list-item-title>
                  </v-list-item>

                  <v-list-item>
                    <template #prepend>
                      <v-icon>mdi-map-marker</v-icon>
                    </template>
                    <v-list-item-title>{{
                      user.city.name.en
                    }}</v-list-item-title>
                  </v-list-item>

                  <v-list-item>
                    <template #prepend>
                      <v-icon>mdi-translate</v-icon>
                    </template>
                    <v-list-item-title>{{
                      user.language === "en" ? "English" : "العربية"
                    }}</v-list-item-title>
                  </v-list-item>
                </v-list>
              </v-card-text>
            </v-card>

            <v-card class="mt-4" variant="outlined">
              <v-card-text>
                <div class="text-subtitle-1 mb-2">إعدادات الجهاز</div>
                <v-divider class="mb-3" />

                <v-list density="compact">
                  <v-list-item>
                    <template #prepend>
                      <v-icon>mdi-devices</v-icon>
                    </template>
                    <v-list-item-title>{{ user.device_id }}</v-list-item-title>
                  </v-list-item>

                  <v-list-item>
                    <template #prepend>
                      <v-icon>mdi-restart</v-icon>
                    </template>
                    <v-list-item-title>
                      {{
                        user.device_restricted ? "مقيد بجهاز واحد" : "غير مقيد"
                      }}
                    </v-list-item-title>
                  </v-list-item>

                  <v-list-item>
                    <template #prepend>
                      <v-icon>mdi-map-marker-radius</v-icon>
                    </template>
                    <v-list-item-title>
                      {{
                        user.location_restricted
                          ? "مقيد بالمكان"
                          : "غير مقيد بالمكان"
                      }}
                    </v-list-item-title>
                  </v-list-item>

                  <v-list-item>
                    <template #prepend>
                      <v-icon>mdi-bell</v-icon>
                    </template>
                    <v-list-item-title>
                      {{
                        user.notifications_enabled
                          ? "الإشعارات مفعلة"
                          : "الإشعارات معطلة"
                      }}
                    </v-list-item-title>
                  </v-list-item>
                </v-list>
              </v-card-text>
            </v-card>
          </v-col>

          <!-- Information Column -->
          <v-col cols="12" md="8">
            <v-card class="mb-6" variant="outlined">
              <v-card-text>
                <div class="text-subtitle-1 mb-2">المعلومات الشخصية</div>
                <v-divider class="mb-3" />

                <v-row>
                  <v-col cols="12" md="6">
                    <v-list-item>
                      <template #prepend>
                        <v-icon>mdi-account</v-icon>
                      </template>
                      <v-list-item-title>الاسم الأول</v-list-item-title>
                      <v-list-item-subtitle class="text-right">{{
                        user.firstName
                      }}</v-list-item-subtitle>
                    </v-list-item>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-list-item>
                      <template #prepend>
                        <v-icon>mdi-account-group</v-icon>
                      </template>
                      <v-list-item-title>اسم العائلة</v-list-item-title>
                      <v-list-item-subtitle class="text-right">{{
                        user.lastName
                      }}</v-list-item-subtitle>
                    </v-list-item>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-list-item>
                      <template #prepend>
                        <v-icon>mdi-identifier</v-icon>
                      </template>
                      <v-list-item-title>رقم الهوية</v-list-item-title>
                      <v-list-item-subtitle class="text-right">{{
                        user.saId
                      }}</v-list-item-subtitle>
                    </v-list-item>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-list-item>
                      <template #prepend>
                        <v-icon>mdi-gender-male-female</v-icon>
                      </template>
                      <v-list-item-title>الجنس</v-list-item-title>
                      <v-list-item-subtitle class="text-right">{{
                        user.gender === "male" ? "ذكر" : "أنثى"
                      }}</v-list-item-subtitle>
                    </v-list-item>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-list-item>
                      <template #prepend>
                        <v-icon>mdi-cake</v-icon>
                      </template>
                      <v-list-item-title>تاريخ الميلاد</v-list-item-title>
                      <v-list-item-subtitle class="text-right">{{
                        formatDate(user.birthDate)
                      }}</v-list-item-subtitle>
                    </v-list-item>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-list-item>
                      <template #prepend>
                        <v-icon>mdi-calendar-plus</v-icon>
                      </template>
                      <v-list-item-title>تاريخ الإنشاء</v-list-item-title>
                      <v-list-item-subtitle class="text-right">{{
                        formatDateTime(user.createdAt)
                      }}</v-list-item-subtitle>
                    </v-list-item>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-list-item>
                      <template #prepend>
                        <v-icon>mdi-calendar-edit</v-icon>
                      </template>
                      <v-list-item-title>تاريخ التعديل</v-list-item-title>
                      <v-list-item-subtitle class="text-right">{{
                        formatDateTime(user.updated_at)
                      }}</v-list-item-subtitle>
                    </v-list-item>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-list-item>
                      <template #prepend>
                        <v-icon>mdi-email-check</v-icon>
                      </template>
                      <v-list-item-title>تاريخ التحقق من البريد</v-list-item-title>
                      <v-list-item-subtitle class="text-right">
                        {{
                          user.email_verified_at
                            ? formatDateTime(user.email_verified_at)
                            : "غير محقق"
                        }}
                      </v-list-item-subtitle>
                    </v-list-item>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-list-item>
                      <template #prepend>
                        <v-icon>mdi-login</v-icon>
                      </template>
                      <v-list-item-title>آخر تسجيل دخول</v-list-item-title>
                      <v-list-item-subtitle class="text-right">
                        {{
                          user.last_login_at
                            ? formatDateTime(user.last_login_at)
                            : "لم يسجل دخول بعد"
                        }}
                      </v-list-item-subtitle>
                    </v-list-item>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-list-item>
                      <template #prepend>
                        <v-icon>mdi-clock-outline</v-icon>
                      </template>
                      <v-list-item-title>ساعات العمل الشهرية</v-list-item-title>
                      <v-list-item-subtitle class="text-right">{{
                        user.monthlyWorkingHours
                      }}
                        ساعة</v-list-item-subtitle>
                    </v-list-item>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-list-item>
                      <template #prepend>
                        <v-icon>mdi-calendar-month</v-icon>
                      </template>
                      <v-list-item-title>أيام العمل</v-list-item-title>
                      <v-list-item-subtitle class="text-right">{{ user.workingDaysCount }} يوم</v-list-item-subtitle>
                    </v-list-item>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-list-item>
                      <template #prepend>
                        <v-icon>mdi-cash</v-icon>
                      </template>
                      <v-list-item-title>المعدل اليومي</v-list-item-title>
                      <v-list-item-subtitle class="text-right">{{ user.dailyRate }} ر.س</v-list-item-subtitle>
                    </v-list-item>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-list-item>
                      <template #prepend>
                        <v-icon>mdi-link-off</v-icon>
                      </template>
                      <v-list-item-title>فصل من الأحداث اليومية</v-list-item-title>
                      <v-list-item-subtitle class="text-right">
                        {{ user.unlink_from_events_daily_base ? "نعم" : "لا" }}
                      </v-list-item-subtitle>
                    </v-list-item>
                  </v-col>
                </v-row>
              </v-card-text>
            </v-card>

            <!-- Attendance Status -->
            <v-card class="mb-6" variant="outlined">
              <v-card-text>
                <div class="text-subtitle-1 mb-2">حالة الحضور اليوم</div>
                <v-divider class="mb-3" />

                <v-row>
                  <v-col cols="12" md="6">
                    <v-list-item>
                      <template #prepend>
                        <v-icon>mdi-login</v-icon>
                      </template>
                      <v-list-item-title>تسجيل الدخول</v-list-item-title>
                      <v-list-item-subtitle class="text-right">
                        {{
                          user.todayCheckIn
                            ? formatDateTime(user.todayCheckIn)
                            : "لم يسجل دخول"
                        }}
                      </v-list-item-subtitle>
                    </v-list-item>
                  </v-col>

                  <v-col cols="12" md="6">
                    <v-list-item>
                      <template #prepend>
                        <v-icon>mdi-logout</v-icon>
                      </template>
                      <v-list-item-title>تسجيل الخروج</v-list-item-title>
                      <v-list-item-subtitle class="text-right">
                        {{
                          user.todayCheckOut
                            ? formatDateTime(user.todayCheckOut)
                            : "لم يسجل خروج"
                        }}
                      </v-list-item-subtitle>
                    </v-list-item>
                  </v-col>

                  <v-col cols="12">
                    <v-list-item>
                      <template #prepend>
                        <v-icon
                          :color="getStatusColor(user.todayAttendanceStatus)"
                        >
                          {{ getStatusIcon(user.todayAttendanceStatus) }}
                        </v-icon>
                      </template>
                      <v-list-item-title>حالة الحضور</v-list-item-title>
                      <v-list-item-subtitle class="text-right">
                        {{ getStatusText(user.todayAttendanceStatus) }}
                      </v-list-item-subtitle>
                    </v-list-item>
                  </v-col>
                </v-row>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script setup lang="ts">
  import { formatDate } from '@/@core/utils/formatters'

  const props = defineProps<{
    user: Record<string, any>
  }>()

  const formatDateTime = (dateTimeString: string) => {
    if (!dateTimeString) return ''
    const options: Intl.DateTimeFormatOptions = {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    }
    return new Date(dateTimeString).toLocaleDateString('ar-SA', options)
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'present':
        return 'success'
      case 'absent':
        return 'error'
      case 'late':
        return 'warning'
      default:
        return 'info'
    }
  }

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'present':
        return 'mdi-check-circle'
      case 'absent':
        return 'mdi-close-circle'
      case 'late':
        return 'mdi-alert-circle'
      default:
        return 'mdi-help-circle'
    }
  }

  const getStatusText = (status: string) => {
    switch (status) {
      case 'present':
        return 'حاضر'
      case 'absent':
        return 'غائب'
      case 'late':
        return 'متأخر'
      default:
        return 'غير محدد'
    }
  }
</script>

<style scoped>
.v-avatar {
  border: 4px solid rgb(var(--v-theme-primary));
}

.text-h5 {
  font-weight: 500;
}

.v-list-item {
  padding-left: 0;
  padding-right: 0;
}

.v-list-item-subtitle {
  opacity: 1;
  font-weight: 500;
}
</style>
