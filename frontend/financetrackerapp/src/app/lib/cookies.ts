import Cookies from 'js-cookie';

const ACCESS_TOKEN_KEY = 'accessToken';
const REFRESH_TOKEN_KEY = 'refreshToken';

export function setAuthTokens(accessToken: string, refreshToken: string): void {
  // Set HTTP-only cookies via API route (more secure)
  // For now, using client-side cookies with secure flags
  Cookies.set(ACCESS_TOKEN_KEY, accessToken, {
    expires: 1, // 1 day
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax',
  });
  Cookies.set(REFRESH_TOKEN_KEY, refreshToken, {
    expires: 7, // 7 days
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax',
  });
}

export function getAccessToken(): string | undefined {
  return Cookies.get(ACCESS_TOKEN_KEY);
}

export function getRefreshToken(): string | undefined {
  return Cookies.get(REFRESH_TOKEN_KEY);
}

export function clearAuthTokens(): void {
  Cookies.remove(ACCESS_TOKEN_KEY);
  Cookies.remove(REFRESH_TOKEN_KEY);
}

