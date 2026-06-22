export type ValidatorResult = string | true

export type ValidatorFn = (value: unknown) => ValidatorResult

export type ValidationRules = Record<string, ValidatorFn | ValidatorFn[]>
