<template>
  <v-dialog
    v-model="detailsDialog"
    max-width="700"
    min-height="calc(100% - 40px)"
  >
    <v-card class="d-block" rounded="lg">
      <!-- Header -->
      <v-card-title
        class="bg-light-gray border-b-md border-grey-900 rounded-t-lg py-4"
      >
        <div
          class="w-100 d-flex justify-end"
          style="position: absolute; left: 10px; top: 10px"
        >
          <button @click="detailsDialog = false">
            <v-icon color="primary" icon="mdi-close" />
          </button>
        </div>
        <div class="text-center">
          <User :size="80" />
          <h5 class="py-2">{{ selectedUser?.fullName }}</h5>
          <div class="d-flex gap-2 justify-center py-1">
            <v-chip class="px-4 text-sm" color="success" size="small">
              {{ selectedUser?.userType?.name?.ar }}
            </v-chip>
            <!--  -->
            <v-chip
              v-if="selectedUser"
              class="px-4 text-sm"
              :color="
                selectedUser?.todayAttendanceStatus == 'withdraw'
                  ? 'success'
                  : 'error'
              "
              :prepend-icon="
                selectedUser?.todayAttendanceStatus == 'withdraw' ? Correct : ''
              "
              size="small"
            >
              <span class="mr-1">
                {{ selectedUser?.todayAttendanceStatus ? STATUSOPTIONS[selectedUser?.todayAttendanceStatus] : "لم يسجل" }}
              </span>
            </v-chip>
            <!--  -->
            <v-chip
              v-if="selectedUser"
              class="px-4 text-sm"
              color="success"
              :prepend-icon="Clock"
              size="small"
            >
              <span class="mr-1">{{
                selectedUser?.todayCheckIn
                  ? formatTimeTo12Hour(selectedUser?.todayCheckIn)
                  : "لم يسجل"
              }}</span>
            </v-chip>
            <!--  -->
            <v-chip
              class="px-4 text-sm"
              :color="selectedUser?.isActive ? 'success' : 'error'"
              size="small"
            >
              {{ selectedUser?.isActive ? "حساب مفعل" : "حساب معطل" }}
            </v-chip>
          </div>
        </div>
      </v-card-title>
      <!-- Present/Absent Status -->
      <v-card-text class="pt-4 px-3 px-lg-6 d-flex justify-center">
        <!-- Attendance Section -->
        <v-tabs v-model="tab" class="rounded-lg" hide-slider>
          <v-tab
            class="rounded-lg bg-light-gray mx-1"
            :class="{ 'bg-light-success text-primary': tab == 'profile' }"
            value="profile"
          >
            الملف الشخصى للمراقب
          </v-tab>
          <v-tab
            class="rounded-lg bg-light-gray mx-1"
            :class="{ 'bg-light-success text-primary': tab == 'attendance' }"
            value="attendance"
          >
            سجل الحضور والغياب
          </v-tab>

          <v-tab
            class="rounded-lg bg-light-gray mx-1"
            :class="{ 'bg-light-success text-primary': tab == 'events' }"
            value="events"
          >
            المواقع
          </v-tab>
          <v-tab
            class="rounded-lg bg-light-gray mx-1"
            :class="{ 'bg-light-success text-primary': tab == 'transfers' }"
            value="transfers"
          >
            سجل الحوالات
          </v-tab>
        </v-tabs>
      </v-card-text>
      <v-card-text class="px-0">
        <v-window v-model="tab">
          <!-- Profile Tab -->
          <v-window-item class="px-0 py-3" value="profile">
            <div class="px-3 px-lg-6">
              <!-- Basic Info -->
              <VCard class="mb-4 px-2 border">
                <VCardTitle class="mb-1 text-primary-text">
                  <h5>المعلومات الأساسية</h5>
                </VCardTitle>
                <v-divider />
                <VCardText class="px-0">
                  <v-row class="mx-0">
                    <v-col cols="12" md="6">
                      <div class="text-caption d-flex align-start gap-2">
                        <span>
                          <Identity />
                        </span>
                        <div class="w-100">
                          <div class="text-sec-text mb-2">رقم الهوية</div>
                          <div v-if="isUserDetailsLoading">
                            <v-skeleton-loader
                              style="width: 100%; max-width: 200px"
                              type="subtitle"
                            />
                          </div>
                          <div v-else class="text-primary-text">
                            {{ selectedUser?.saId }}
                          </div>
                        </div>
                      </div>
                    </v-col>
                    <v-col cols="12" md="6">
                      <div class="text-caption d-flex align-start gap-2">
                        <span>
                          <RealstateIcon />
                        </span>
                        <div class="w-100">
                          <div class="text-sec-text mb-2">المدينة</div>
                          <div v-if="isUserDetailsLoading">
                            <v-skeleton-loader
                              style="width: 100%; max-width: 200px"
                              type="subtitle"
                            />
                          </div>
                          <div v-else class="text-primary-text">
                            {{ selectedUser?.city?.name?.ar }}
                          </div>
                        </div>
                      </div>
                    </v-col>
                    <v-col cols="12" md="6">
                      <div class="text-caption d-flex align-start gap-2">
                        <span>
                          <Call />
                        </span>
                        <div class="w-100">
                          <div class="text-sec-text mb-2">رقم الجوال</div>
                          <div v-if="isUserDetailsLoading">
                            <v-skeleton-loader
                              style="width: 100%; max-width: 200px"
                              type="subtitle"
                            />
                          </div>
                          <div v-else class="text-primary-text">
                            {{ selectedUser?.phone }}
                          </div>
                        </div>
                      </div>
                    </v-col>
                    <v-col cols="12" md="6">
                      <div class="text-caption d-flex align-start gap-2">
                        <span>
                          <Calender />
                        </span>
                        <div class="w-100">
                          <div class="text-sec-text mb-2">تاريخ الميلاد</div>
                          <div v-if="isUserDetailsLoading">
                            <v-skeleton-loader
                              style="width: 100%; max-width: 200px"
                              type="subtitle"
                            />
                          </div>
                          <div v-else class="text-primary-text">
                            {{ selectedUser?.birthDate || "غير محدد" }}
                          </div>
                        </div>
                      </div>
                    </v-col>
                    <v-col cols="12">
                      <div class="text-caption d-flex align-start gap-2">
                        <span>
                          <Mail />
                        </span>
                        <div class="w-100">
                          <div class="text-sec-text mb-2">
                            البريد الإلكتروني
                          </div>
                          <div v-if="isUserDetailsLoading">
                            <v-skeleton-loader
                              style="width: 100%; max-width: 200px"
                              type="subtitle"
                            />
                          </div>
                          <div
                            v-else
                            class="text-primary-text ellipsis email-address"
                          >
                            {{ selectedUser?.email }}
                          </div>
                        </div>
                      </div>
                    </v-col>

                    <v-col cols="12" md="6">
                      <div class="text-caption d-flex align-start gap-2">
                        <span>
                          <Gender />
                        </span>
                        <div class="w-100">
                          <div class="text-sec-text mb-2">الجنس</div>
                          <div v-if="isUserDetailsLoading">
                            <v-skeleton-loader
                              style="width: 100%; max-width: 200px"
                              type="subtitle"
                            />
                          </div>
                          <div v-else class="text-primary-text">
                            {{
                              selectedUser?.gender === "male" ? "ذكر" : "أنثى"
                            }}
                          </div>
                        </div>
                      </div>
                    </v-col>
                    <v-col cols="12" md="6">
                      <div class="text-caption d-flex align-start gap-2">
                        <span>
                          <Money />
                        </span>
                        <div class="w-100">
                          <div class="text-sec-text mb-2">الأجر اليومى</div>
                          <div v-if="isUserDetailsLoading">
                            <v-skeleton-loader
                              style="width: 100%; max-width: 200px"
                              type="subtitle"
                            />
                          </div>
                          <div v-else class="text-primary-text">
                            {{ selectedUser?.dailyRate }}
                            <Currency />
                          </div>
                        </div>
                      </div>
                    </v-col>
                  </v-row>
                </VCardText>
              </VCard>
              <!--  -->
              <VCard class="mb-4 px-2 border">
                <VCardTitle class="mb-1">
                  <h5>المعلومات المصرفية</h5>
                </VCardTitle>
                <v-divider />
                <VCardText class="px-0">
                  <v-row class="mx-0">
                    <v-col cols="12" md="6">
                      <div class="text-caption d-flex align-start gap-2">
                        <span>
                          <Bank />
                        </span>

                        <div class="w-100">
                          <div class="text-sec-text mb-2">اسم البنك</div>
                          <div v-if="isUserDetailsLoading">
                            <v-skeleton-loader
                              style="width: 100%; max-width: 200px"
                              type="subtitle"
                            />
                          </div>
                          <div
                            v-else
                            class="text-primary-text d-flex align-center justify-space-between"
                          >
                            <span class="ellipsis bank-info-text">
                              {{ selectedUser?.bankInfo?.bankName }}
                            </span>
                            <v-btn
                              :icon="Copy"
                              size="small"
                              variant="text"
                              @click="
                                copyEvent(selectedUser?.bankInfo?.bankName)
                              "
                            />
                          </div>
                        </div>
                      </div>
                    </v-col>
                    <v-col cols="12" md="6">
                      <div class="text-caption d-flex align-start gap-2">
                        <span>
                          <Credit />
                        </span>
                        <div class="w-100">
                          <div class="text-sec-text mb-2">
                            رقم الحساب البنكي
                          </div>
                          <div v-if="isUserDetailsLoading">
                            <v-skeleton-loader
                              style="width: 100%; max-width: 200px"
                              type="subtitle"
                            />
                          </div>
                          <div
                            v-else
                            class="text-primary-text d-flex align-center justify-space-between"
                          >
                            <span class="ellipsis bank-info-text">
                              {{ selectedUser?.bankInfo?.accountNumber }}
                            </span>
                            <v-btn
                              :icon="Copy"
                              size="small"
                              variant="text"
                              @click="
                                copyEvent(selectedUser?.bankInfo?.accountNumber)
                              "
                            />
                          </div>
                        </div>
                      </div>
                    </v-col>
                    <v-col cols="12" md="6">
                      <div class="text-caption d-flex align-start gap-2">
                        <span>
                          <CreditB />
                        </span>
                        <div class="w-100">
                          <div class="text-sec-text mb-2">
                            رقم الآيبان (IBAN)
                          </div>
                          <div v-if="isUserDetailsLoading">
                            <v-skeleton-loader
                              style="width: 100%; max-width: 200px"
                              type="subtitle"
                            />
                          </div>
                          <div
                            v-else
                            class="text-primary-text d-flex align-center justify-space-between"
                          >
                            <span class="ellipsis bank-info-text">
                              {{ selectedUser?.bankInfo?.iban }}
                            </span>
                            <v-btn
                              :icon="Copy"
                              size="small"
                              variant="text"
                              @click="copyEvent(selectedUser?.bankInfo?.iban)"
                            />
                          </div>
                        </div>
                      </div>
                    </v-col>
                    <v-col cols="12" md="6">
                      <div class="text-caption d-flex align-start gap-2">
                        <span>
                          <SignH />
                        </span>
                        <div class="w-100">
                          <div class="text-sec-text mb-2">
                            رمز السويفت (SWIFT)
                          </div>
                          <div v-if="isUserDetailsLoading">
                            <v-skeleton-loader
                              style="width: 100%; max-width: 200px"
                              type="subtitle"
                            />
                          </div>
                          <div
                            v-else
                            class="text-primary-text d-flex align-center justify-space-between"
                          >
                            <span class="ellipsis bank-info-text">
                              {{ selectedUser?.bankInfo?.swiftCode }}
                            </span>
                            <v-btn
                              :icon="Copy"
                              size="small"
                              variant="text"
                              @click="
                                copyEvent(selectedUser?.bankInfo?.swiftCode)
                              "
                            />
                          </div>
                        </div>
                      </div>
                    </v-col>
                  </v-row>
                </VCardText>
              </VCard>
            </div>
          </v-window-item>

          <!-- Attendance Tab -->
          <v-window-item class="px-0 py-3" value="attendance">
            <div class="px-4 px-lg-6">
              <div v-if="isUserAttendacesLoading && isUserDetailsLoading">
                <v-skeleton-loader type="article" />
              </div>
              <div v-else-if="userAttendaces.length">
                <div
                  v-for="(group, gIdx) in groupAttendancesByMonth"
                  :key="gIdx"
                  class="px-0"
                >
                  <div class="mb-2 text-primary-text">
                    {{ group.monthName }} {{ group.year }}
                  </div>
                  <VCard
                    v-for="(attendance, aIdx) in group.attendances"
                    :key="aIdx"
                    class="border mb-3"
                    rounded="lg"
                  >
                    <VCardText class="px-3 py-3">
                      <span
                        class="workhours-badge px-2 py-1 rounded-pill text-xs font-bold d-flex align-center ga-1"
                      >
                        <Clock /> {{ parseInt(attendance?.totalHours ?? 0) }}
                      </span>
                      <div
                        class="d-flex gap-3 align-center day-card rounded-lg mb-3"
                      >
                        <div
                          class="d-flex flex-column justify-center align-center day rounded-lg bg-background"
                        >
                          <span class="d-block">
                            {{ getDay(attendance.date) }}
                          </span>
                          <span class="d-block">
                            {{ group.monthName }} {{ group.year }}
                          </span>
                        </div>
                        <div class="text">
                          <div class="mb-2">
                            <span class="text-primary-text font-bold">
                              وقت الحضور :
                            </span>

                            <span class="text-sec-text">
                              {{ formatTimeTo12Hour(attendance.checkIn) }}
                            </span>
                          </div>
                          <div>
                            <span class="text-primary-text font-bold">
                              وقت الإنصراف :
                            </span>

                            <span class="text-sec-text">
                              {{
                                attendance.checkOut
                                  ? formatTimeTo12Hour(attendance.checkOut)
                                  : "لم يسجل"
                              }}
                            </span>
                          </div>
                        </div>
                      </div>
                      <v-divider />
                      <div class="d-flex align-center ga-2 pt-3">
                        <v-img
                          v-if="attendance?.location?.image"
                          height="30"
                          max-width="30"
                          :src="attendance?.location?.image"
                          width="30"
                        />
                        <v-img
                          v-else
                          height="30"
                          max-width="30"
                          src="/imgs/location-icon.png"
                          width="30"
                        />
                        <p
                          class="text-xs font-bold py-3 mb-0 text-primary-text"
                        >
                          {{ attendance?.location?.name }}
                        </p>
                      </div>
                    </VCardText>
                  </VCard>
                </div>

                <br>
                <div class="d-flex justify-center">
                  <v-btn
                    v-if="!isLastAttendacePage"
                    class="font-bold"
                    color="light-gray"
                    :loading="isUserAttendacesLoading"
                    size="small"
                    variant="flat"
                    @click="handleLoadMoreAttendances"
                  >
                    عرض المزيد
                  </v-btn>
                </div>
              </div>

              <div v-else class="text-center py-5">
                <p>لا يوجد سجل حضور وغياب</p>
              </div>
            </div>
          </v-window-item>

          <!-- events tab -->
          <v-window-item class="px-0 py-3" value="events">
            <div class="px-4 px-lg-6">
              <v-skeleton-loader v-if="isUserDetailsLoading" type="article" />

              <template v-else-if="selectedUser?.locations?.length">
                <v-card
                  v-for="loc in selectedUser.locations"
                  :key="loc?.id"
                  class="border border-light mb-2"
                  rounded="lg"
                >
                  <VCardTitle class="d-flex align-center ga-2">
                    <span>
                      <v-img v-if="loc.image" :src="loc.image" width="40" />
                      <v-img v-else src="/imgs/location-icon.png" width="40" />
                    </span>
                    <div class="d-flex flex-column">
                      <h5 class="mb-0">{{ loc.name }}</h5>
                      <span class="text-sm text-sec-text">
                        📅
                        {{
                          getPeriod(loc.assignedAt, loc.unassignedAt) ||
                            "غير محدد"
                        }}
                      </span>
                    </div>
                  </VCardTitle>
                  <v-divider />
                  <VCardText class="px-2 py-4">
                    <v-row class="mx-0">
                      <v-col class="d-flex gap-2" cols="12" sm="6">
                        <span>
                          <Job />
                        </span>
                        <div class="text-xs font-bold">
                          <div class="text-sec-text mb-2">الدور</div>
                          <div class="text-primary-text">
                            {{ selectedUser?.userType?.name?.ar }}
                          </div>
                        </div>
                      </v-col>
                      <v-col class="d-flex gap-2" cols="12" sm="6">
                        <span>
                          <Clock />
                        </span>
                        <div class="text-xs font-bold">
                          <div class="text-sec-text mb-2">
                            ساعات العمل في الموقع
                          </div>
                          <div class="text-primary-text">
                            {{ loc.totalWorkingHours ?? "غير محدد" }} ساعة
                          </div>
                        </div>
                      </v-col>
                    </v-row>
                  </VCardText>
                </v-card>
              </template>

              <div v-else class="py-5 text-center">
                <p class="text-center">لا توجد مواقع</p>
              </div>
            </div>
          </v-window-item>

          <!-- transfers tab -->
          <v-window-item class="px-0 py-3" value="transfers">
            <div class="px-4 px-lg-6">
              <v-skeleton-loader v-if="isUserDetailsLoading" type="article" />

              <div v-else-if="userTransactions?.length">
                <div
                  v-for="group in groupTransactionsByMonthYear"
                  :key="`${group.year}-${group.month}`"
                  class="mb-4"
                >
                  <h6 class="text-sm mb-3">
                    {{ group.monthName }} {{ group.year }}
                  </h6>

                  <div class="transfers-container">
                    <div
                      v-for="(transfer, tIdx) in group.transactions"
                      :key="tIdx"
                      class="transfer-item"
                    >
                      <v-card class="border" elevation="5">
                        <v-card-text
                          class="d-flex gap-3 align-center day-card b rounded-lg py-2 px-2"
                        >
                          <div
                            class="d-flex flex-column justify-center align-center day rounded-lg bg-light-gray"
                          >
                            <span class="d-block">
                              {{ getDay(transfer.transactionDate) }}
                            </span>
                            <span class="d-block">
                              {{ group.monthName }} {{ group.year }}
                            </span>
                          </div>
                          <div v-if="transfer.type === 'payment'" class="text">
                            <div class="mb-2">
                              <span class="text-primary-text font-bold">
                                نوع الحوالة :
                              </span>
                              <span class="">
                                {{ transfer.paymentType?.name }}
                              </span>
                            </div>
                            <div>
                              <span class="text-primary-text font-bold">
                                المبلغ :
                              </span>
                              <span class="text-primary font-bold text-lg">
                                {{ transfer.amount }}
                                <Currency />
                              </span>
                            </div>
                          </div>
                          <div v-else class="text">
                            <div class="mb-2">
                              <span class="text-primary-text font-bold">
                                نوع الخصم :
                              </span>
                              <span class="">
                                خصم
                                {{ DEDUCTIONTYPES[transfer.deductionBasis] }}
                              </span>
                            </div>
                            <div class="mb-2">
                              <span class="text-primary-text font-bold">
                                المدة :
                              </span>
                              <span class="text-primary">
                                {{ transfer.deductionValue }}
                              </span>
                            </div>
                            <div>
                              <span class="text-primary-text font-bold">
                                المبلغ :
                              </span>
                              <span class="text-primary font-bold text-lg">
                                {{ transfer.amount }}
                                <Currency />
                              </span>
                            </div>
                          </div>
                        </v-card-text>
                        <v-divider />
                        <v-card-actions>
                          <div>
                            <div v-if="transfer.description">
                              <p class="text-xs py-3 mb-0">
                                <b class="text-primary-text">السبب: </b>
                                {{ transfer.description }}
                              </p>
                            </div>
                          </div>
                        </v-card-actions>
                      </v-card>
                    </div>
                  </div>
                </div>
                <br>
                <div class="d-flex justify-center">
                  <v-btn
                    v-if="!isLastTransactionsPage"
                    class="font-bold"
                    color="light-gray"
                    :loading="isUserTransLoading"
                    size="small"
                    variant="flat"
                    @click="handleLoadMoreTransactions"
                  >
                    عرض المزيد
                  </v-btn>
                </div>
              </div>

              <div v-else class="py-5 text-center">
                <p>لا يوجد سجل حوالات</p>
              </div>
            </div>
          </v-window-item>
        </v-window>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>
<script setup lang="ts">
  import Correct from './icons/correct.vue'
  import User from '@/@core/components/icons/user.vue'
  import Clock from './icons/clock.vue'
  import Identity from './icons/identity.vue'
  import RealstateIcon from './icons/realstateIcon.vue'
  import Call from './icons/call.vue'
  import Mail from './icons/mail.vue'
  import Calender from './icons/calender.vue'
  import Gender from './icons/gender.vue'
  import Money from './icons/money.vue'
  import Bank from './icons/bank.vue'
  import Credit from './icons/credit.vue'
  import CreditB from './icons/credit-b.vue'
  import SignH from './icons/sign-h.vue'
  import Job from './icons/job.vue'
  import Copy from './icons/copy.vue'
  import Currency from './icons/currency.vue'
  import { useAppStore } from '@/stores/app'
  import { computed, onMounted, watch } from 'vue'
  import {
    formatDateToMonthShort,
    formatTimeTo12Hour,
  } from '@/@core/utils/formatters'
  import { attendanceApi, financialApi } from '@/api'

  const appStore = useAppStore()

  interface AttendanceGroup {
    year: number
    month: number
    monthName: string
    attendances: any[]
  }

  interface TransactionGroup {
    year: number
    month: number
    monthName: string
    transactions: any[]
  }

  const props = defineProps<{
    selectedUser: Record<string, any> | null
    isUserDetailsLoading: boolean
  }>()
  const detailsDialog = defineModel<boolean>({ required: true })
  const tab = ref('profile')
  const STATUSOPTIONS: Record<string, string> = {
    absent: 'غائب',
    attendance: 'حاضر',
    departed: 'غادر',
  }
  const DEDUCTIONTYPES: Record<string, string> = { days: 'أيام', hours: 'ساعات' }

  const copyEvent = async (textToCopy: string) => {
    try {
      await navigator.clipboard.writeText(textToCopy)
      appStore.showSnackbar({
        message: 'Copied',
        color: 'primary',
      })
    } catch (err: unknown) {
      appStore.showSnackbar({
        message: 'فشل نسخ هذا النص !',
        color: 'error',
      })
    }
  }

  const userAttendaces = ref<any[]>([])
  const isUserAttendacesLoading = ref(false)
  const attendanceLoadCount = ref(10)
  const attendanceLoadMoreCurrentPage = ref(1)
  const isLastAttendacePage = ref(false)
  const fetchUserAttendances = async () => {
    try {
      isUserAttendacesLoading.value = true
      const response = await attendanceApi.getList({
        user_id: props.selectedUser?.id,
        per_page: attendanceLoadCount.value,
        page: attendanceLoadMoreCurrentPage.value,
      })

      if (attendanceLoadMoreCurrentPage.value === 1) {
        userAttendaces.value = response.data?.data
      } else {
        userAttendaces.value = userAttendaces.value.concat(
          response.data?.data ?? [],
        )
      }
      const pagination = response.data?.pagination
      const totalPages = Math.ceil(
        (pagination?.iTotalObjects ?? 0) /
          (pagination?.iPerPage ?? attendanceLoadCount.value),
      )
      isLastAttendacePage.value =
        totalPages <= attendanceLoadMoreCurrentPage.value
    } catch (error: unknown) {
    } finally {
      isUserAttendacesLoading.value = false
    }
  }

  const userTransactions = ref<any[]>([])
  const isUserTransLoading = ref(false)
  const transactionsLoadCount = ref(10)
  const transactionsLoadMoreCurrentPage = ref(1)
  const isLastTransactionsPage = ref(false)
  const fetchUserTransactions = async () => {
    try {
      isUserTransLoading.value = true
      const response = await financialApi.getByUser(props.selectedUser?.id, {
        per_page: transactionsLoadCount.value,
        page: transactionsLoadMoreCurrentPage.value,
      })

      if (transactionsLoadMoreCurrentPage.value === 1) {
        userTransactions.value = response.data?.data
      } else {
        userTransactions.value = userTransactions.value.concat(
          response.data?.data ?? [],
        )
      }
      const txPagination = response.data?.pagination
      const txTotalPages = Math.ceil(
        (txPagination?.iTotalObjects ?? 0) /
          (txPagination?.iPerPage ?? transactionsLoadCount.value),
      )
      isLastTransactionsPage.value =
        txTotalPages <= transactionsLoadMoreCurrentPage.value
    } catch (error: unknown) {
    } finally {
      isUserTransLoading.value = false
    }
  }

  watch(
    () => detailsDialog.value,
    newValue => {
      if (newValue && props.selectedUser?.id) {
        tab.value = 'profile'
        attendanceLoadMoreCurrentPage.value = 1
        fetchUserAttendances()
        transactionsLoadMoreCurrentPage.value = 1
        fetchUserTransactions()
      }
    },
    { deep: true },
  )

  const handleLoadMoreAttendances = () => {
    attendanceLoadMoreCurrentPage.value++
    fetchUserAttendances()
  }
  const handleLoadMoreTransactions = () => {
    transactionsLoadMoreCurrentPage.value++
    fetchUserTransactions()
  }

  const groupAttendancesByMonth = computed(() => {
    const grouped: Record<string, AttendanceGroup> = {}

    userAttendaces.value.forEach((attendance: any) => {
      const date = new Date(attendance.date)
      const year = date.getFullYear()
      const month = date.getMonth() + 1

      const key = `${year}-${month}`

      if (!grouped[key]) {
        grouped[key] = {
          year,
          month,
          monthName: date.toLocaleString('ar-EG', { month: 'long' }),
          attendances: [],
        }
      }

      grouped[key].attendances.push(attendance)
    })

    const result = Object.values(grouped).sort((a, b) => {
      if (a.year !== b.year) return b.year - a.year
      return b.month - a.month
    })

    return result
  })

  const groupTransactionsByMonthYear = computed(() => {
    const groupedData: Record<string, TransactionGroup> = {}

    userTransactions.value.forEach((transaction: any) => {
      const date = new Date(transaction.transactionDate)
      const year = date.getFullYear()
      const month = date.getMonth() + 1

      const monthName = date.toLocaleString('ar-EG', { month: 'long' })

      const key = `${year}-${month}`

      if (!groupedData[key]) {
        groupedData[key] = {
          year,
          month,
          monthName,
          transactions: [],
        }
      }

      groupedData[key].transactions.push(transaction)
    })

    const result = Object.values(groupedData).sort((a, b) => {
      if (a.year !== b.year) return b.year - a.year
      return b.month - a.month
    })

    return result
  })

  const getDay = (StringDate: string) => {
    const date = new Date(StringDate)
    return date.getDate()
  }

  const DayMonth = (stringDate: string) => {
    const date = new Date(stringDate)
    return date.toLocaleString('ar-EG', { month: 'long', day: 'numeric' })
  }

  const getPeriod = (start: string, end: string) => {
    if (!start || !end) return 'المدة غيرر محددة'
    const startYear = new Date(start).getFullYear()
    const endYear = new Date(end).getFullYear()
    if (startYear === endYear) {
      return `من ${DayMonth(start)} إلى ${DayMonth(end)} ${endYear}`
    }
    return `من ${DayMonth(start)} إلى ${DayMonth(end)} ${startYear}-${endYear}`
  }
</script>
<style lang="scss" scoped>
.workhours-badge {
  position: absolute;
  left: 20px;
  background-color: rgb(var(--v-theme-light-gray));
}

.bank-info-text {
  max-width: 200px;
  direction: ltr;
  text-align: end;
}

.transfers-container {
  display: flex;
  flex-wrap: wrap;
  gap: 15px;
}

.transfer-item {
  flex: 1 1 310px;
  min-width: 0;
}
</style>
