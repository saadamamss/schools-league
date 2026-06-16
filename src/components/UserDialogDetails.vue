<template>
  <v-dialog
    v-model="details_dialog"
    max-width="700"
    min-height="calc(100% - 40px)"
  >
    <v-card rounded="lg" class="d-block">
      <!-- Header -->
      <v-card-title
        class="bg-light-gray border-b-md border-grey-900 rounded-t-lg py-4"
      >
        <div
          class="w-100 d-flex justify-end"
          style="position: absolute; left: 10px; top: 10px"
        >
          <button @click="details_dialog = false">
            <v-icon icon="mdi-close" color="primary"> </v-icon>
          </button>
        </div>
        <div class="text-center">
          <User :size="80" />
          <h5 class="py-2">{{ selectedUser?.full_name }}</h5>
          <div class="d-flex gap-2 justify-center py-1">
            <v-chip class="px-4 text-sm" color="success" size="small">
              {{ selectedUser?.user_type?.name?.ar }}
            </v-chip>
            <v-chip
              v-if="today_attendance_status"
              class="px-4 text-sm"
              :color="
                selectedUser?.today_attendance_status == 'withdraw'
                  ? 'success'
                  : 'error'
              "
              size="small"
              :prepend-icon="
                selectedUser?.today_attendance_status == 'withdraw'
                  ? Correct
                  : ''
              "
            >
              <span class="mr-1">
                {{ STATUSOPTIONS[selectedUser?.today_attendance_status] }}
              </span>
            </v-chip>

            <v-chip
              v-if="selectedUser?.today_check_in"
              class="px-4 text-sm"
              color="success"
              size="small"
              :prepend-icon="Clock"
            >
              <span class="mr-1">{{
                selectedUser?.today_check_in
                  ? formatTimeTo12Hour(selectedUser?.today_check_in)
                  : "لم يسجل"
              }}</span>
            </v-chip>
            <v-chip
              class="px-4 text-sm"
              :color="selectedUser?.is_active ? 'success' : 'error'"
              size="small"
            >
              {{ selectedUser?.is_active ? "حساب مفعل" : "حساب معطل" }}
            </v-chip>
          </div>
        </div>
      </v-card-title>
      <!-- Present/Absent Status -->
      <v-card-text class="pt-4 px-3 px-lg-6 d-flex justify-center">
        <!-- Attendance Section -->
        <v-tabs v-model="tab" class="rounded-lg" hide-slider>
          <v-tab
            value="profile"
            class="rounded-lg bg-light-gray mx-1"
            :class="{ 'bg-light-success text-primary': tab == 'profile' }"
          >
            الملف الشخصى للمراقب
          </v-tab>
          <v-tab
            value="attendance"
            class="rounded-lg bg-light-gray mx-1"
            :class="{ 'bg-light-success text-primary': tab == 'attendance' }"
          >
            سجل الحضور والغياب
          </v-tab>

          <v-tab
            value="events"
            class="rounded-lg bg-light-gray mx-1"
            :class="{ 'bg-light-success text-primary': tab == 'events' }"
          >
            الفعاليات
          </v-tab>
          <v-tab
            value="transfers"
            class="rounded-lg bg-light-gray mx-1"
            :class="{ 'bg-light-success text-primary': tab == 'transfers' }"
          >
            سجل الحوالات
          </v-tab>
        </v-tabs>
      </v-card-text>
      <v-card-text class="px-0">
        <v-window v-model="tab">
          <!-- Profile Tab -->
          <v-window-item value="profile" class="px-0 py-3">
            <div class="px-3 px-lg-6">
              <!-- Basic Info -->
              <VCard class="mb-4 px-2 border">
                <VCardTitle class="mb-1 text-primary-text">
                  <h5>المعلومات الأساسية</h5>
                </VCardTitle>
                <v-divider></v-divider>
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
                              type="subtitle"
                              style="width: 100%; max-width: 200px"
                            ></v-skeleton-loader>
                          </div>
                          <div v-else class="text-primary-text">
                            {{ selectedUser?.sa_id }}
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
                              type="subtitle"
                              style="width: 100%; max-width: 200px"
                            ></v-skeleton-loader>
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
                              type="subtitle"
                              style="width: 100%; max-width: 200px"
                            ></v-skeleton-loader>
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
                              type="subtitle"
                              style="width: 100%; max-width: 200px"
                            ></v-skeleton-loader>
                          </div>
                          <div v-else class="text-primary-text">
                            {{ selectedUser?.birth_date || "غير محدد" }}
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
                              type="subtitle"
                              style="width: 100%; max-width: 200px"
                            ></v-skeleton-loader>
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
                              type="subtitle"
                              style="width: 100%; max-width: 200px"
                            ></v-skeleton-loader>
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
                              type="subtitle"
                              style="width: 100%; max-width: 200px"
                            ></v-skeleton-loader>
                          </div>
                          <div v-else class="text-primary-text">
                            {{ selectedUser?.daily_rate }}
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
                <v-divider></v-divider>
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
                              type="subtitle"
                              style="width: 100%; max-width: 200px"
                            ></v-skeleton-loader>
                          </div>
                          <div
                            v-else
                            class="text-primary-text d-flex align-center justify-space-between"
                          >
                            <span class="ellipsis bank-info-text">
                              {{ selectedUser?.bank_info?.bank_name }}
                            </span>
                            <v-btn
                              @click="
                                copyEvent(selectedUser?.bank_info?.bank_name)
                              "
                              variant="text"
                              :icon="Copy"
                              size="small"
                            >
                            </v-btn>
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
                              type="subtitle"
                              style="width: 100%; max-width: 200px"
                            ></v-skeleton-loader>
                          </div>
                          <div
                            v-else
                            class="text-primary-text d-flex align-center justify-space-between"
                          >
                            <span class="ellipsis bank-info-text">
                              {{ selectedUser?.bank_info?.account_number }}
                            </span>
                            <v-btn
                              @click="
                                copyEvent(
                                  selectedUser?.bank_info?.account_number
                                )
                              "
                              variant="text"
                              :icon="Copy"
                              size="small"
                            >
                            </v-btn>
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
                              type="subtitle"
                              style="width: 100%; max-width: 200px"
                            ></v-skeleton-loader>
                          </div>
                          <div
                            v-else
                            class="text-primary-text d-flex align-center justify-space-between"
                          >
                            <span class="ellipsis bank-info-text">
                              {{ selectedUser?.bank_info?.iban }}
                            </span>
                            <v-btn
                              @click="copyEvent(selectedUser?.bank_info?.iban)"
                              variant="text"
                              :icon="Copy"
                              size="small"
                            >
                            </v-btn>
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
                              type="subtitle"
                              style="width: 100%; max-width: 200px"
                            ></v-skeleton-loader>
                          </div>
                          <div
                            v-else
                            class="text-primary-text d-flex align-center justify-space-between"
                          >
                            <span class="ellipsis bank-info-text">
                              {{ selectedUser?.bank_info?.swift_code }}
                            </span>
                            <v-btn
                              @click="
                                copyEvent(selectedUser?.bank_info?.swift_code)
                              "
                              variant="text"
                              :icon="Copy"
                              size="small"
                            >
                            </v-btn>
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
          <v-window-item value="attendance" class="px-0 py-3">
            <div class="px-4 px-lg-6">
              <div v-if="isUserAttendacesLoading && isUserDetailsLoading">
                <v-skeleton-loader type="article"></v-skeleton-loader>
              </div>
              <div v-else-if="userAttendaces.length">
                <div class="px-0" v-for="group in groupAttendancesByMonth">
                  <div class="mb-2 text-primary-text">
                    {{ group.monthName }} {{ group.year }}
                  </div>
                  <VCard
                    v-for="attendance in group.attendances"
                    class="border mb-3"
                    rounded="lg"
                  >
                    <VCardText class="px-3 py-3">
                      <span
                        class="workhours-badge px-2 py-1 rounded-pill text-xs font-bold d-flex align-center ga-1"
                      >
                        <Clock /> {{ parseInt(attendance?.total_hours) }}
                      </span>
                      <div
                        class="d-flex gap-3 align-center day-card rounded-lg mb-3"
                      >
                        <div
                          class="d-flex flex-column justify-center align-center day rounded-lg bg-background"
                        >
                          <span class="d-block">
                            {{ getDay(attendance.check_in_date) }}
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
                              {{ formatTime(attendance.check_in_time) }}
                            </span>
                          </div>
                          <div>
                            <span class="text-primary-text font-bold">
                              وقت الإنصراف :
                            </span>

                            <span class="text-sec-text">
                              {{
                                attendance.check_out_time
                                  ? formatTime(attendance.check_out_time)
                                  : "لم يسجل"
                              }}
                            </span>
                          </div>
                        </div>
                      </div>
                      <v-divider></v-divider>
                      <div class="d-flex align-center ga-2 pt-3">
                        <v-img
                          v-if="attendance?.location?.image"
                          :src="attendance?.location?.image"
                          max-width="30"
                          width="30"
                          height="30"
                        />
                        <v-img
                          v-else
                          src="/imgs/event-icon.png"
                          max-width="30"
                          width="30"
                          height="30"
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

                <br />
                <div class="d-flex justify-center">
                  <v-btn
                    variant="flat"
                    color="light-gray"
                    size="small"
                    class="font-bold"
                    @click="handleLoadMoreAttendances"
                    :loading="isUserAttendacesLoading"
                    v-if="!isLastAttendacePage"
                  >
                    عرض المزيد
                  </v-btn>
                </div>
              </div>

              <div class="text-center py-5" v-else>
                <p>لا يوجد سجل حضور وغياب</p>
              </div>
            </div>
          </v-window-item>

          <!-- events tab -->
          <v-window-item value="events" class="px-0 py-3">
            <div class="px-4 px-lg-6">
              <v-skeleton-loader
                v-if="isUserDetailsLoading"
                type="article"
              ></v-skeleton-loader>

              <v-card
                v-else-if="selectedUser?.location"
                rounded="lg"
                class="border border-light mb-2"
                v-for="event in [selectedUser.location]"
              >
                <VCardTitle class="d-flex align-center ga-2">
                  <span>
                    <v-img v-if="event.image" :src="event.image" width="40" />
                    <v-img v-else src="/imgs/event-icon.png" width="40" />
                  </span>
                  <div class="d-flex flex-column">
                    <h5 class="mb-0">{{ event.name }}</h5>
                    <span class="text-sm text-sec-text">
                      📅 {{ getPeriod(event.start_date, event.end_date) }}
                    </span>
                  </div>
                </VCardTitle>
                <v-divider></v-divider>
                <VCardText class="px-2 py-4">
                  <v-row class="mx-0">
                    <v-col cols="12" sm="6" class="d-flex gap-2">
                      <span>
                        <Job />
                      </span>
                      <div class="text-xs font-bold">
                        <div class="text-sec-text mb-2">الدور</div>
                        <div class="text-primary-text">
                          {{ selectedUser?.user_type?.name?.ar }}
                        </div>
                      </div>
                    </v-col>
                    <v-col cols="12" sm="6" class="d-flex gap-2">
                      <span>
                        <Clock />
                      </span>
                      <div class="text-xs font-bold">
                        <div class="text-sec-text mb-2">
                          ساعات العمل فى الفعالية
                        </div>
                        <div class="text-primary-text">
                          {{ event.total_working_hours }} ساعة
                        </div>
                      </div>
                    </v-col>
                  </v-row>
                </VCardText>
              </v-card>

              <div v-else class="py-5 text-center">
                <p class="text-center">لا توجد فعاليات</p>
              </div>
            </div>
          </v-window-item>

          <!-- transfers tab -->
          <v-window-item value="transfers" class="px-0 py-3">
            <div class="px-4 px-lg-6">
              <v-skeleton-loader
                v-if="isUserDetailsLoading"
                type="article"
              ></v-skeleton-loader>

              <div v-else-if="userTransactions?.length">
                <div v-for="group in groupTransactionsByMonthYear" class="mb-4">
                  <h6 class="text-sm mb-3">
                    {{ group.monthName }} {{ group.year }}
                  </h6>

                  <div class="transfers-container">
                    <div
                      v-for="transfer in group.transactions"
                      class="transfer-item"
                    >
                      <v-card elevation="5" class="border">
                        <v-card-text
                          class="d-flex gap-3 align-center day-card b rounded-lg py-2 px-2"
                        >
                          <div
                            class="d-flex flex-column justify-center align-center day rounded-lg bg-light-gray"
                          >
                            <span class="d-block">
                              {{ getDay(transfer.transaction_date) }}
                            </span>
                            <span class="d-block">
                              {{ group.monthName }} {{ group.year }}
                            </span>
                          </div>
                          <div class="text" v-if="transfer.type == 'payment'">
                            <div class="mb-2">
                              <span class="text-primary-text font-bold">
                                نوع الحوالة :
                              </span>
                              <span class="">
                                {{ transfer.payment_type?.name?.ar }}
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
                          <div class="text" v-else>
                            <div class="mb-2">
                              <span class="text-primary-text font-bold">
                                نوع الخصم :
                              </span>
                              <span class="">
                                خصم
                                {{ DEDUCTIONTYPES[transfer.deduction_basis] }}
                              </span>
                            </div>
                            <div class="mb-2">
                              <span class="text-primary-text font-bold">
                                المدة :
                              </span>
                              <span class="text-primary">
                                {{ transfer.deduction_value }}
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
                        <v-divider></v-divider>
                        <v-card-actions>
                          <div>
                            <div v-if="transfer.notes">
                              <p class="text-xs py-3 mb-0">
                                <b class="text-primary-text">السبب: </b>
                                {{ transfer.notes }}
                              </p>
                            </div>
                            <div
                              class="d-flex align-center ga-2 pt-3"
                              v-for="location in transfer.locations"
                            >
                              <v-img
                                src="/imgs/event-icon.png"
                                max-width="30"
                                width="30"
                                height="30"
                              ></v-img>

                              <p
                                class="text-xs font-bold py-3 mb-0 text-primary-text"
                              >
                                {{ location?.name }}
                              </p>
                            </div>
                          </div>
                        </v-card-actions>
                      </v-card>
                    </div>
                  </div>
                </div>
                <br />
                <div class="d-flex justify-center">
                  <v-btn
                    variant="flat"
                    color="light-gray"
                    size="small"
                    class="font-bold"
                    @click="handleLoadMoreTransactions"
                    :loading="isUserTransLoading"
                    v-if="!isLastTransactionsPage"
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
<script setup>
import Correct from "./icons/correct.vue";
import User from "@/@core/components/icons/user.vue";
import Clock from "./icons/clock.vue";
import Identity from "./icons/identity.vue";
import RealstateIcon from "./icons/realstateIcon.vue";
import Call from "./icons/call.vue";
import Mail from "./icons/mail.vue";
import Calender from "./icons/calender.vue";
import Gender from "./icons/gender.vue";
import Money from "./icons/money.vue";
import Bank from "./icons/bank.vue";
import Credit from "./icons/credit.vue";
import CreditB from "./icons/credit-b.vue";
import SignH from "./icons/sign-h.vue";
import Job from "./icons/job.vue";
import Copy from "./icons/copy.vue";
import Currency from "./icons/currency.vue";
import { useAppStore } from "@/stores/app";
import { computed, onMounted, watch } from "vue";
import {
  formatDateToMonthShort,
  formatTime,
  formatTimeTo12Hour,
} from "@/@core/utils/formatters";
import axiosIns from "@/plugins/axios";

const appStore = useAppStore();
const props = defineProps(["selectedUser", "isUserDetailsLoading"]);
const details_dialog = defineModel();
const tab = ref("profile");
const STATUSOPTIONS = { absent: "غائب", withdraw: "حاضر", departed: "غادر" };
const DEDUCTIONTYPES = { days: "أيام", hours: "ساعات" };

const copyEvent = async function (textToCopy) {
  try {
    await navigator.clipboard.writeText(textToCopy);
    appStore.showSnackbar({
      message: "Copied",
      color: "primary",
    });
  } catch (err) {
    appStore.showSnackbar({
      message: "فشل نسخ هذا النص !",
      color: "error",
    });
  }
};

const userAttendaces = ref([]);
const isUserAttendacesLoading = ref(false);
const attendanceLoadCount = ref(10);
const attendanceLoadMoreCurrentPage = ref(1);
const isLastAttendacePage = ref(false);
const fetchUserAttendances = async () => {
  try {
    isUserAttendacesLoading.value = true;
    const response = await axiosIns.get(
      `users/${props.selectedUser?.id}/attendance`,
      {
        params: {
          per_page: attendanceLoadCount.value,
          page: attendanceLoadMoreCurrentPage.value,
        },
      }
    );

    if (attendanceLoadMoreCurrentPage.value === 1) {
      userAttendaces.value = response.data?.data;
    } else {
      userAttendaces.value = userAttendaces.value.concat(response.data?.data);
    }
    isLastAttendacePage.value =
      response.data?.pagination?.i_total_pages ==
      attendanceLoadMoreCurrentPage.value;
  } catch (error) {
  } finally {
    isUserAttendacesLoading.value = false;
  }
};

const userTransactions = ref([]);
const isUserTransLoading = ref(false);
const transactionsLoadCount = ref(10);
const transactionsLoadMoreCurrentPage = ref(1);
const isLastTransactionsPage = ref(false);
const fetchUserTransactions = async () => {
  try {
    isUserTransLoading.value = true;
    const response = await axiosIns.get(
      `users/${props.selectedUser?.id}/financial-transaction`,
      {
        params: {
          per_page: transactionsLoadCount.value,
          page: transactionsLoadMoreCurrentPage.value,
        },
      }
    );

    if (transactionsLoadMoreCurrentPage.value === 1) {
      userTransactions.value = response.data?.data;
    } else {
      userTransactions.value = userTransactions.value.concat(
        response.data?.data
      );
    }
    isLastTransactionsPage.value =
      response.data?.pagination?.i_total_pages ==
      attendanceLoadMoreCurrentPage.value;
  } catch (error) {
  } finally {
    isUserTransLoading.value = false;
  }
};

watch(
  () => details_dialog.value,
  (newValue) => {
    if (newValue && props.selectedUser.id) {
      tab.value = "profile";
      attendanceLoadMoreCurrentPage.value = 1;
      fetchUserAttendances();
      transactionsLoadMoreCurrentPage.value = 1;
      fetchUserTransactions();
    }
  },
  { deep: true }
);

const handleLoadMoreAttendances = () => {
  attendanceLoadMoreCurrentPage.value++;
  fetchUserAttendances();
};
const handleLoadMoreTransactions = () => {
  transactionsLoadMoreCurrentPage.value++;
  fetchUserTransactions();
};

const groupAttendancesByMonth = computed(() => {
  const grouped = {};

  userAttendaces.value.forEach((attendance) => {
    const date = new Date(attendance.check_in_date);
    const year = date.getFullYear();
    const month = date.getMonth() + 1; // الأشهر من 0-11 لذا نضيف 1

    // مفتاح التجميع بصيغة "سنة-شهر" مثلاً "2025-6"
    const key = `${year}-${month}`;

    // إذا لم يكن الشهر موجوداً، ننشئ مصفوفة جديدة له
    if (!grouped[key]) {
      grouped[key] = {
        year: year,
        month: month,
        monthName: date.toLocaleString("ar-EG", { month: "long" }), // اسم الشهر بالعربية
        attendances: [],
      };
    }

    // نضيف الحضور إلى مصفوفة الشهر المناسب
    grouped[key].attendances.push(attendance);
  });

  // نحول الكائن إلى مصفوفة ونسّيقها حسب التاريخ الأحدث
  const result = Object.values(grouped).sort((a, b) => {
    if (a.year !== b.year) return b.year - a.year;
    return b.month - a.month;
  });

  return result;
});

//

const groupTransactionsByMonthYear = computed(() => {
  const groupedData = {};

  userTransactions.value.forEach((transaction) => {
    const date = new Date(transaction.transaction_date);
    const year = date.getFullYear();
    const month = date.getMonth() + 1; // لأن الأشهر تبدأ من 0

    // اسم الشهر بالعربية
    const monthName = date.toLocaleString("ar-EG", { month: "long" });

    const key = `${year}-${month}`;

    if (!groupedData[key]) {
      groupedData[key] = {
        year: year,
        month: month,
        monthName: monthName,
        transactions: [],
      };
    }

    groupedData[key].transactions.push(transaction);
  });

  // تحويل الكائن إلى مصفوفة وترتيبها من الأحدث إلى الأقدم
  const result = Object.values(groupedData).sort((a, b) => {
    if (a.year !== b.year) return b.year - a.year;
    return b.month - a.month;
  });

  return result;
});

const getDay = (StringDate) => {
  const date = new Date(StringDate);
  return date.getDate();
};

const DayMonth = (stringDate) => {
  const date = new Date(stringDate);
  return date.toLocaleString("ar-EG", { month: "long", day: "numeric" });
};

const getPeriod = (start, end) => {
  if (!start || !end) return "المدة غيرر محددة";
  const startYear = new Date(start).getFullYear();
  const endYear = new Date(end).getFullYear();
  if (startYear == endYear) {
    return `من ${DayMonth(start)} إلى ${DayMonth(end)} ${endYear}`;
  }
  return `من ${DayMonth(start)} إلى ${DayMonth(end)} ${startYear}-${endYear}`;
};
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
