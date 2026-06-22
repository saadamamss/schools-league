<script setup lang="ts">
  import Logo from '@/@core/components/icons/logo.vue'
</script>

<template>
  <div class="login-warpper bg-surface">
    <div class="overlayer">
      <!-- <svg
        width="1440"
        height="981"
        viewBox="0 0 1440 981"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          opacity="0.21"
          d="M-1260.94 970.481C-1260.94 434.79 -826.678 0.525421 -290.987 0.525421H624.831C1160.52 0.525421 1594.79 434.789 1594.79 970.48V2035.18C1594.79 2040.16 1590.75 2044.2 1585.76 2044.2H-1251.92C-1256.9 2044.2 -1260.94 2040.16 -1260.94 2035.18V970.481Z"
          fill="#3E8774"
        />
        <path
          opacity="0.21"
          d="M-1416.59 1141.92C-1416.59 606.224 -982.322 171.959 -446.631 171.959H469.187C1004.88 171.959 1439.14 606.223 1439.14 1141.91V2206.61C1439.14 2211.59 1435.1 2215.63 1430.12 2215.63H-1407.56C-1412.55 2215.63 -1416.59 2211.59 -1416.59 2206.61V1141.92Z"
          fill="#519382"
        />
        <path
          opacity="0.21"
          d="M-1567.72 1360.72C-1567.72 825.027 -1133.45 390.763 -597.763 390.763H318.054C853.746 390.763 1288.01 825.026 1288.01 1360.72V2425.41C1288.01 2430.4 1283.97 2434.44 1278.99 2434.44H-1558.7C-1563.68 2434.44 -1567.72 2430.4 -1567.72 2425.41V1360.72Z"
          fill="#629E8E"
        />
        <path
          opacity="0.21"
          d="M-1682.76 1543.43C-1682.76 1007.74 -1248.5 573.476 -712.805 573.476H203.013C738.704 573.476 1172.97 1007.74 1172.97 1543.43V2608.13C1172.97 2613.11 1168.93 2617.15 1163.95 2617.15H-1673.74C-1678.72 2617.15 -1682.76 2613.11 -1682.76 2608.13V1543.43Z"
          fill="#72A899"
        />
        <path
          opacity="0.21"
          d="M-1671.48 1726.14C-1671.48 1190.45 -1237.22 756.188 -701.526 756.188H214.292C749.983 756.188 1184.25 1190.45 1184.25 1726.14V2790.84C1184.25 2795.82 1180.21 2799.86 1175.22 2799.86H-1662.46C-1667.44 2799.86 -1671.48 2795.82 -1671.48 2790.84V1726.14Z"
          fill="#80B1A3"
        />
      </svg> -->
    </div>
    <v-container class="h-100" fluid>
      <div class="h-100 m-0 px-0 py-0 d-flex flex-column justify-space-between">
        <div
          class="w-100 d-flex align-center justify-space-between pb-3 pt-6 px-4"
        >
          <Logo />
        </div>
        <div class="content px-4">
          <RouterView />
        </div>

        <div class="py-5" style="position: relative; z-index: 10">
          <div class="d-flex justify-center align-center ga-5">
            <Logo />
          </div>
        </div>
      </div>
    </v-container>
  </div>
</template>
<style lang="scss" scoped>
.v-theme--light {
  .login-warpper {
    background-color: #f7fffd !important;
  }
}
.login-warpper {
  min-height: 100vh;
  position: relative;
  overflow: hidden;
  .overlayer {
    position: absolute;
    // bottom: -485px;
    top: 40vh;
    bottom: 0px;
    left: 0px;
    width: 100%;
    z-index: 0;
    overflow: hidden;
    svg {
      width: 100%;
      min-width: 1350px;
      // height: 100% !important;
      @media (max-width: 768px) {
        transform: translate(252px, 0px);
      }
    }
  }
  .content {
    width: 100%;
    max-width: 845px;
    margin: 0 auto;

    :deep(.v-input) {
      position: relative !important;
      // div{
      //   background-color: #fff !important;
      // }
      // input.v-field__input {
      //   background-color: #fff !important;
      // }
      // .v-field__outline{
      //   border-radius: 16px !important;
      //   box-shadow: 0px 3px 222px rgba(0, 0, 0, 0.12);
      // }

      .v-field.v-field--focused .v-field__outline {
        // color: #252525 !important;
        --v-field-border-width: 1px !important;
        box-shadow: 0 0 0 4px #4848484f !important;
      }
      .v-field.v-field--error .v-field__outline {
        box-shadow: 0 0 0 4px #ee61614f !important;
      }
    }
    :deep(.v-input__details) {
      position: absolute;
      bottom: -25px;
      padding-inline: 2px;
      right: 0px;
      .v-messages__message {
        font-size: 14px;
      }
    }
  }
}
</style>
