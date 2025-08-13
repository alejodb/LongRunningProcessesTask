import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react"

type CountTextOcurrencesRequest = {
  text: string
  connectionId: string | null
}

type CountTextOcurrencesResponse = {
  processId: string
}

export const textProcessorApiSlice = createApi({
  baseQuery: fetchBaseQuery({ baseUrl: "http://localhost:5281/" }),
  reducerPath: "textProcessorApi",
  endpoints: (builder) => ({
    countTextOccurrences: builder.mutation<CountTextOcurrencesResponse, CountTextOcurrencesRequest>({
      query: ({ text, connectionId }) => {
        return {
          url: "/api/textprocessor",
          method: "POST",
          body: {
            text,
            connectionId
          }
        }
      },
    }),
    cancelProcess: builder.mutation<undefined, string>({
      query: (processId) => {
        return {
          url: `/api/textprocessor/${processId}`,
          method: "DELETE"
        }
      },
    }),
  }),
})

export const { useCountTextOccurrencesMutation, useCancelProcessMutation } = textProcessorApiSlice
