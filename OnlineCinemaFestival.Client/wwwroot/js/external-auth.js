let googleScriptPromise;
let appleScriptPromise;

function loadScript(src, id) {
  const existing = document.getElementById(id);
  if (existing) {
    return Promise.resolve();
  }

  return new Promise((resolve, reject) => {
    const script = document.createElement("script");
    script.id = id;
    script.src = src;
    script.async = true;
    script.defer = true;
    script.onload = resolve;
    script.onerror = () => reject(new Error("Nao foi possivel carregar o provider externo."));
    document.head.appendChild(script);
  });
}

async function signInGoogle(clientId, returnUrl) {
  googleScriptPromise ??= loadScript(
    "https://accounts.google.com/gsi/client",
    "google-identity-services"
  );
  await googleScriptPromise;

  return new Promise((resolve, reject) => {
    const tokenClient = google.accounts.oauth2.initTokenClient({
      client_id: clientId,
      scope: "openid email profile",
      callback: response => {
        if (response.error) {
          reject(new Error(response.error_description || response.error));
          return;
        }

        resolve({
          provider: "Google",
          idToken: "",
          accessToken: response.access_token || "",
          returnUrl: returnUrl || ""
        });
      }
    });

    tokenClient.requestAccessToken({ prompt: "select_account" });
  });
}

async function signInApple(clientId, returnUrl) {
  appleScriptPromise ??= loadScript(
    "https://appleid.cdn-apple.com/appleauth/static/jsapi/appleid/1/en_US/appleid.auth.js",
    "apple-signin-js"
  );
  await appleScriptPromise;

  AppleID.auth.init({
    clientId,
    scope: "name email",
    redirectURI: window.location.origin,
    usePopup: true
  });

  const response = await AppleID.auth.signIn();

  return {
    provider: "Apple",
    idToken: response?.authorization?.id_token || "",
    accessToken: response?.authorization?.code || "",
    returnUrl: returnUrl || ""
  };
}

export async function signIn(provider, clientId, returnUrl) {
  if (!clientId) {
    throw new Error("Provider externo sem ClientId configurado.");
  }

  if (provider === "Google") {
    return signInGoogle(clientId, returnUrl);
  }

  if (provider === "Apple") {
    return signInApple(clientId, returnUrl);
  }

  throw new Error("Provider externo nao suportado.");
}
