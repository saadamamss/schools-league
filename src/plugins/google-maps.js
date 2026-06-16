// Google Maps Configuration
const GOOGLE_MAPS_API_KEY = import.meta.env.VITE_GOOGLE_MAPS_API_KEY;

export const googleMapsConfig = {
  apiKey: GOOGLE_MAPS_API_KEY,
  // Add any additional Google Maps configuration options here
  libraries: ["places"], // Enable places library by default
};

export const loadGoogleMapsScript = () => {
  return new Promise((resolve, reject) => {
    // Check if Google Maps is already loaded
    if (window.google && window.google.maps) {
      resolve(window.google.maps);
      return;
    }

    // Create script element
    const script = document.createElement("script");
    script.src = `https://maps.googleapis.com/maps/api/js?key=${GOOGLE_MAPS_API_KEY}&libraries=places`;
    script.async = true;
    script.defer = true;

    // Handle script load event
    script.addEventListener("load", () => {
      resolve(window.google.maps);
    });

    // Handle script error event
    script.addEventListener("error", () => {
      reject(new Error("Failed to load Google Maps script"));
    });

    // Append script to document
    document.head.appendChild(script);
  });
};

export default {
  install: (app) => {
    app.config.globalProperties.$googleMapsConfig = googleMapsConfig;
    app.config.globalProperties.$loadGoogleMaps = loadGoogleMapsScript;
  },
};
