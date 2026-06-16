import axios from "@/plugins/axios";
import { toast } from "vue3-toastify";

export const saveToStorage = (key, value) => {
  if (!key || !value) return null;

  localStorage.setItem(key, JSON.stringify(value));
};

export const getFromStorage = (key) => {
  if (!key) return null;

  const value = localStorage.getItem(key);

  return value ? JSON.parse(value) : null;
};

export const removeFromStorage = (key) => {
  if (!key) return null;

  localStorage.removeItem(key);
};

export function printElement(elementId, title = "تقييم") {
  const printContents = document.getElementById(elementId).innerHTML;
  const printWindow = window.open("", "", "width=800,height=600");
  printWindow.document.write(`
    <html>
      <head>
        <title>${title}</title>
        <link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=IBM+Plex+Sans+Arabic:wght@100;200;300;400;500;600;700&family=Inter:ital,opsz,wght@0,14..32,100..900;1,14..32,100..900&display=swap" rel="stylesheet">
        <style>
        .no-print {
            display: none;
          }

          .print-title {
            font-size: 24px;
            font-weight: bold;
          }
          body {
            direction: rtl;
            text-align: right;
            font-family: "IBMPlexArabic", Tahoma, sans-serif;
            padding: 20px;
          }
          .mt-4 {
              margin-top: 16px !important;
          }
          .v-row {
              display: flex;
              flex-wrap: wrap;
              flex: 1 1 auto;
              margin: -12px;
          }

          .d-flex {
              display: flex !important;
          }
          .ga-2 {
              gap: 8px !important;
          }
          .ga-1 {
              gap: 4px !important;
          }
          .ga-4 {
              gap: 16px !important;
          }
          .align-center {
              align-items: center !important;
          }

          .mb-6 {
              margin-bottom: 24px !important;
          }

          .mt-4 {
              margin-top: 16px !important;
          }

          .v-col-md-4 {
              flex: 0 0 33.3333333333%;
              max-width: 33.3333333333%;
          }

          .ps-10 {
              padding-inline-start: 40px !important;
          }

          .w-100 {
              width: 100% !important;
          }

          .align-start {
              align-items: flex-start !important;
          }

          .justify-space-between {
              justify-content: space-between !important;
          }

          .flex-wrap {
              flex-wrap: wrap !important;
          }

          a {
              color: #000;
              text-decoration: none;
          }
          .font-weight-bold {
              font-weight: 700 !important;
          }

          .v-chip.v-chip--density-default {
            height: 26px;
          }

          .v-col-md-6 {
              flex: 0 0 50%;
              width: 100%;
              max-width: calc(50% - 2rem);
              padding: 12px;
          }

          .text-body-1 {
              font-size: 1rem !important;
              font-weight: 400;
              line-height: 1.5;
              letter-spacing: 0.03125em !important;
              font-family: "IBMPlexArabic", Tahoma, sans-serif;
              text-transform: none !important;
          }

          h1, h2, h3, h4, h5, h6, .text-h1, .text-h2, .text-h3, .text-h4, .text-h5, .text-h6, .text-button, .text-overline, .v-card-title {
              line-height: 1.8;
              font-family: "IBMPlexArabic", Tahoma, sans-serif;
          }

          .text-h6 {
              font-size: 1.25rem !important;
              font-weight: 500;
              line-height: 1.5rem;
              letter-spacing: 0.0125em !important;
              text-transform: none !important;
          }

          .mb-2 {
              margin-bottom: 8px !important;
          }

        </style>
      </head>
      <body>${printContents}</body>
    </html>
  `);
  printWindow.document.close();
  printWindow.focus();
  printWindow.print();
  printWindow.close();
}

export const exportData = async (url, filters = {}, successMessage) => {
  return await new Promise(async (resolve, reject) => {
    try {
      const response = await axios.post(url, filters, {
        responseType: "blob",
      });

      const responseMessage = response.data?.status?.message || "";
      if (successMessage || responseMessage) {
        toast.success(successMessage || responseMessage, {
          rtl: true,
          hideProgressBar: true,
          position: "top-center",
        });
      }

      const fileURL = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = fileURL;

      // Optional: Get file name from headers
      const disposition = response.headers["content-disposition"];

      let fileName = "datatable-records.xlsx";
      if (disposition && disposition.includes("filename=")) {
        fileName = disposition
          .split("filename=")[1]
          .replace(/['"]/g, "")
          .trim();
      }

      link.setAttribute("download", fileName);
      document.body.appendChild(link);
      link.click();

      // Clean up
      link.remove();
      setTimeout(() => {
        window.URL.revokeObjectURL(fileURL);
      }, 100);
      resolve(response.data);
    } catch (error) {
      console.error("Download failed:", error);
      toast.error("خطأ غير متوقع اثناء التصدير!", {
        rtl: true,
        hideProgressBar: true,
        position: "top-center",
      });
      reject(error);
    }
  });
};

export const exportPDFFromURL = async (fileUrl, filters = {}, pdfName) => {
  return await new Promise(async (resolve, reject) => {
    try {
      const response = await axios.post(
        fileUrl,
        {},
        {
          responseType: "blob", // Important to receive binary data
          ...filters,
        }
      );

      const url = window.URL.createObjectURL(new Blob([response.data]));

      const link = document.createElement("a");
      link.href = url;

      //Optional: Get file name from headers
      const disposition = response.headers["content-disposition"];
      let fileName = `${pdfName || "file"}.pdf`;
      if (disposition && disposition.includes("filename=")) {
        fileName = disposition
          .split("filename=")[1]
          .replace(/['"]/g, "")
          .trim();
      }

      link.setAttribute("download", fileName);
      document.body.appendChild(link);
      link.click();

      // Clean up
      link.remove();
      window.URL.revokeObjectURL(url);

      resolve(true);
    } catch (error) {
      console.error("Download failed:", error);
      toast.error("خطأ غير متوقع اثناء التصدير!", {
        rtl: true,
        hideProgressBar: true,
        position: "top-center",
      });
      reject(error);
    }
  });
};
