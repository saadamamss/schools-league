/**
 * plugins/index.js
 *
 * Automatically included in `./src/main.js`
 */

// Plugins
import vuetify from "./vuetify";
import pinia from "@/stores";
import router from "@/router";
import googleMaps from "./google-maps";
import VueTelInput from 'vue-tel-input';
import 'vue-tel-input/vue-tel-input.css';

export function registerPlugins(app) {
  app.use(vuetify).use(VueTelInput).use(pinia).use(router).use(googleMaps);
}
