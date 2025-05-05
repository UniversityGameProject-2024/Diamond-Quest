
const functions = require("firebase-functions");
const { google } = require("googleapis");
const admin = require("firebase-admin");
const serviceAccount = require("./service-account.json");

admin.initializeApp();

const sheets = google.sheets("v4");

const auth = new google.auth.GoogleAuth({
  credentials: serviceAccount,
  scopes: ["https://www.googleapis.com/auth/spreadsheets"],
});

// Replace with your actual spreadsheet ID
const SPREADSHEET_ID = 1y11dpIGpXUv39BRckGIH1pNwMKS3d4aSqDsEjwBgTV0;

exports.writeScoreToSheets = functions.database
  .ref("/users/{username}/history/{pushId}")
  .onCreate(async (snapshot, context) => {
    const data = snapshot.val();
    const username = context.params.username;
    const score = data.score;
    const timestamp = data.timestamp;

    const authClient = await auth.getClient();

    await sheets.spreadsheets.values.append({
      auth: authClient,
      spreadsheetId: SPREADSHEET_ID,
      range: "Sheet1!A:C",
      valueInputOption: "USER_ENTERED",
      requestBody: {
        values: [[username, score, new Date(timestamp).toLocaleString()]],
      },
    });

    console.log(`✔️ Score of ${score} by ${username} sent to Google Sheets`);
    return null;
  });
