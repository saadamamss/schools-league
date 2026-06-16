<script setup>
import { avatarText } from "@core/utils/formatters";

const props = defineProps({
  notifications: {
    type: Array,
    required: false,
    default: () => [],
  },
  badgeProps: {
    type: null,
    required: false,
    default: undefined,
  },
  location: {
    type: null,
    required: false,
    default: "bottom end",
  },
});

const emit = defineEmits(["click:readAllNotifications"]);
</script>

<template>
  <VBtn height="48" color="background" class="btn-icon" elevation="0">
    <VBadge
      :model-value="!!props.notifications?.length || 3"
      color="primary"
      content="3"
    >
      <svg
        width="24"
        height="24"
        viewBox="0 0 24 24"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          fill-rule="evenodd"
          clip-rule="evenodd"
          d="M10.5288 1.62434C10.9739 1.33296 11.5172 1.25 12.0002 1.25C12.4832 1.25 13.0265 1.33296 13.4716 1.62434C13.9654 1.94759 14.2502 2.47083 14.2502 3.125C14.2502 3.58918 14.109 4.06938 13.8791 4.48431C17.0316 5.28651 19.4124 8.06135 19.5908 11.4516C19.5999 11.6241 19.6066 11.7893 19.6131 11.9485C19.6353 12.49 19.6546 12.9615 19.7527 13.4133C19.87 13.9531 20.0869 14.3775 20.5281 14.7084C21.2974 15.2854 21.7502 16.191 21.7502 17.1527C21.7502 18.534 20.663 19.75 19.2002 19.75H15.6752C15.3277 21.4617 13.8144 22.75 12.0002 22.75C10.186 22.75 8.67266 21.4617 8.32521 19.75H4.8002C3.33739 19.75 2.2502 18.534 2.2502 17.1527C2.2502 16.191 2.70298 15.2854 3.47233 14.7084C3.91349 14.3775 4.13045 13.9531 4.24769 13.4133C4.3458 12.9615 4.36509 12.49 4.38725 11.9485C4.39376 11.7894 4.40052 11.6241 4.4096 11.4516C4.58804 8.06135 6.96877 5.28651 10.1213 4.48431C9.89145 4.06938 9.7502 3.58918 9.7502 3.125C9.7502 2.47083 10.035 1.94759 10.5288 1.62434ZM9.87823 19.75C10.1871 20.6239 11.0205 21.25 12.0002 21.25C12.9799 21.25 13.8133 20.6239 14.1222 19.75H9.87823ZM19.2002 18.25C19.7738 18.25 20.2502 17.7677 20.2502 17.1527C20.2502 16.6631 20.0197 16.2021 19.6281 15.9084C18.822 15.3039 18.4576 14.5176 18.2869 13.7316C18.1582 13.1391 18.1327 12.4942 18.1105 11.9342C18.105 11.7936 18.0996 11.6581 18.0929 11.5304C17.9223 8.28999 15.2451 5.75 12.0002 5.75C8.75527 5.75 6.07808 8.28999 5.90753 11.5304C5.9008 11.6582 5.89545 11.7935 5.88988 11.9342C5.86772 12.4942 5.84219 13.1391 5.71352 13.7316C5.54282 14.5176 5.1784 15.3039 4.37233 15.9084C3.98069 16.2021 3.7502 16.6631 3.7502 17.1527C3.7502 17.7677 4.22661 18.25 4.8002 18.25H19.2002ZM12.4589 3.92575C12.2649 4.18683 12.0871 4.25 12.0002 4.25C11.9133 4.25 11.7355 4.18683 11.5415 3.92575C11.353 3.67217 11.2502 3.35726 11.2502 3.125C11.2502 3.02106 11.2706 2.97026 11.2818 2.94888C11.2925 2.92864 11.31 2.90574 11.3503 2.87934C11.4481 2.81533 11.6548 2.75 12.0002 2.75C12.3456 2.75 12.5523 2.81533 12.6501 2.87934C12.6904 2.90574 12.7079 2.92864 12.7185 2.94888C12.7298 2.97026 12.7502 3.02106 12.7502 3.125C12.7502 3.35726 12.6474 3.67217 12.4589 3.92575Z"
          fill="#667178"
        />
      </svg>
    </VBadge>

    <VMenu
      activator="parent"
      width="380px"
      :location="props.location"
      offset="14px"
    >
      <VList class="py-0 text-dark">
        <!-- 👉 Header -->
        <VListItem title="التنبيهات" class="notification-section" height="48px">
          <!-- <template #append>
            <VChip color="primary" size="small"> 2 New </VChip>
          </template> -->
        </VListItem>

        <VDivider />

        <!-- 👉 Notifications list -->
        <template
          v-for="notification in props.notifications"
          :key="notification.title"
        >
          <VListItem
            :title="notification.title"
            :subtitle="notification.subtitle"
            link
            lines="one"
            min-height="60px"
            class="text-dark"
          >
            <!-- Slot: Prepend -->
            <!-- Handles Avatar: Image, Icon, Text -->
            <template #prepend>
              <VListItemAction start>
                <div>
                  <svg
                    width="3.5rem"
                    height="3.2rem"
                    viewBox="0 0 28 29"
                    fill="none"
                    xmlns="http://www.w3.org/2000/svg"
                  >
                    <rect
                      y="0.5"
                      width="28"
                      height="28"
                      rx="4"
                      fill="#e8e4df"
                    />
                    <mask
                      id="mask0_20_50273"
                      style="mask-type: luminance"
                      maskUnits="userSpaceOnUse"
                      x="4"
                      y="4"
                      width="20"
                      height="21"
                    >
                      <path
                        d="M23.65 24.15V4.85H4.35V24.15H23.65Z"
                        fill="white"
                        stroke="white"
                        stroke-width="0.7"
                      />
                    </mask>
                    <g mask="url(#mask0_20_50273)">
                      <path
                        d="M9.25344 10.2832H4.29297"
                        stroke="#667178"
                        stroke-width="0.7"
                        stroke-miterlimit="10"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                      />
                      <path
                        d="M23.7066 10.2832H18.7461"
                        stroke="#667178"
                        stroke-width="0.7"
                        stroke-miterlimit="10"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                      />
                      <path
                        d="M22.7447 12.2831C22.7447 11.9519 22.4763 11.6835 22.1451 11.6835H19.1916C18.448 11.6835 17.7658 11.2708 17.4206 10.6122C17.0754 9.95352 16.3932 9.54086 15.6496 9.54086H12.3509C11.6073 9.54086 10.9251 9.95352 10.5799 10.6122C10.2347 11.2708 9.55254 11.6835 8.80891 11.6835H5.85543C5.52426 11.6835 5.25586 11.9519 5.25586 12.2831C5.25586 12.9533 5.7791 13.5069 6.44824 13.5447L10.5011 13.7735C10.903 13.7962 11.2866 13.9487 11.5944 14.208C11.9296 14.4905 12.3539 14.6455 12.7924 14.6455H15.2081C15.6466 14.6455 16.0709 14.4905 16.4062 14.208C16.7139 13.9487 17.0976 13.7962 17.4994 13.7735L21.5523 13.5447C22.2214 13.5069 22.7447 12.9533 22.7447 12.2831Z"
                        stroke="#667178"
                        stroke-width="0.7"
                        stroke-miterlimit="10"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                      />
                      <path
                        d="M16.9428 13.8916L16.2184 13.4361C16.0002 13.299 15.7478 13.2262 15.4901 13.2262H12.5054C12.2477 13.2262 11.9953 13.299 11.7771 13.4361L11.0527 13.8916"
                        stroke="#667178"
                        stroke-width="0.7"
                        stroke-miterlimit="10"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                      />
                      <path
                        d="M7.74011 13.6176L6.73058 19.2731C6.68933 19.5044 6.86714 19.7168 7.1021 19.7168C7.26027 19.7168 7.40167 19.6182 7.45628 19.4697L9.57179 13.721"
                        stroke="#667178"
                        stroke-width="0.7"
                        stroke-miterlimit="10"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                      />
                      <path
                        d="M20.3997 14.4014L20.2598 13.6176"
                        stroke="#667178"
                        stroke-width="0.7"
                        stroke-miterlimit="10"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                      />
                      <path
                        d="M18.4277 13.721L20.5432 19.4698C20.5979 19.6182 20.7393 19.7168 20.8974 19.7168C21.1323 19.7168 21.3102 19.5044 21.2689 19.2731L20.6411 15.7562"
                        stroke="#667178"
                        stroke-width="0.7"
                        stroke-miterlimit="10"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                      />
                      <path
                        d="M7.42473 11.6836V9.93547C7.42473 9.57547 7.13289 9.28367 6.77289 9.28367C6.41293 9.28367 6.12109 9.57547 6.12109 9.93547V11.6836"
                        stroke="#667178"
                        stroke-width="0.7"
                        stroke-miterlimit="10"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                      />
                      <path
                        d="M21.8779 11.6836V9.93547C21.8779 9.57547 21.586 9.28367 21.226 9.28367C20.8661 9.28367 20.5742 9.57547 20.5742 9.93547V11.6836"
                        stroke="#667178"
                        stroke-width="0.7"
                        stroke-miterlimit="10"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                      />
                      <path
                        d="M11.6035 17.1964V16.8266C11.6035 16.44 11.9169 16.1266 12.3035 16.1266H15.6993C16.0858 16.1266 16.3992 16.44 16.3992 16.8266V18.9387C16.3992 19.3253 16.0858 19.6387 15.6993 19.6387H12.3035C11.9169 19.6387 11.6035 19.3253 11.6035 18.9387V18.5662"
                        stroke="#667178"
                        stroke-width="0.7"
                        stroke-miterlimit="10"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                      />
                      <path
                        d="M14.7066 17.883C14.7066 18.2729 14.3906 18.5889 14.0008 18.5889C13.6109 18.5889 13.2949 18.2729 13.2949 17.883C13.2949 17.4932 13.6109 17.1773 14.0008 17.1773C14.3906 17.1773 14.7066 17.4932 14.7066 17.883Z"
                        stroke="#667178"
                        stroke-width="0.7"
                        stroke-miterlimit="10"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                      />
                      <path
                        d="M14.9316 16.127V14.6458"
                        stroke="#667178"
                        stroke-width="0.7"
                        stroke-miterlimit="10"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                      />
                      <path
                        d="M13.0684 14.6458V16.127"
                        stroke="#667178"
                        stroke-width="0.7"
                        stroke-miterlimit="10"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                      />
                    </g>
                  </svg>
                </div>
              </VListItemAction>
            </template>
          </VListItem>
          <VDivider />
        </template>

        <VListItem class="notification-section">
          <VBtn color="dark" variant="text" block> عرض جميع التنبيهات </VBtn>
        </VListItem>
      </VList>
    </VMenu>
  </VBtn>
</template>
