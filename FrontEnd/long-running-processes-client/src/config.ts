type EnvConfig = {
  VITE_API_URL: string;
}
const env: EnvConfig = {
  // eslint-disable-next-line @typescript-eslint/no-unsafe-assignment  
  VITE_API_URL: import.meta.env.VITE_API_URL ?? '/api',
};

export const API_URL = env.VITE_API_URL;