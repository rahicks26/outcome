module Outcome.Validation.SimpleTypes

open System

/// Models a list of errors that may not be
/// homogenesis.
type ErrorList = private ErrorList of obj list

type ErrorListMessageConfig =
    { Deliminator: string option
      SummaryMessage: string option
      EmptyMessage: string option }

[<Literal>]
let DefaultErrorMessageDeliminator = "/n/t-"

[<Literal>]
let DefaultErrorMessageSummaryMessage =
    "The following errors have been found:\n"

[<Literal>]
let DefaultErrorMessageEmptyMessage = ""

let defaultErrorMessageConfig =
    { Deliminator = Some DefaultErrorMessageDeliminator
      SummaryMessage = Some DefaultErrorMessageSummaryMessage
      EmptyMessage = Some DefaultErrorMessageEmptyMessage }

module ErrorList =

    /// Converts the error list to a list of
    /// objects.
    let toList (ErrorList errs) = errs

    /// Creates an error list from an error
    /// or collection of errors. Collections
    /// will be flattened by default. To avoid this
    /// wrap them in another list
    let fromError error =
        match box error with
        | :? ErrorList as error -> error
        | :? unit -> [] |> ErrorList
        | :? (seq<obj>) as errors -> errors |> List.ofSeq |> ErrorList
        | error -> error |> List.singleton |> ErrorList

    /// Appends an error to the error list
    let cons err (ErrorList errs) =
        err |> fromError |> toList |> List.append errs

    /// Appends the first error list to the second
    let append (ErrorList errs1) (ErrorList errs2) = errs1 |> List.append errs2 |> ErrorList

    /// Filters an error list down to those
    /// with a matching type.
    let ofType (ErrorList errs) =
        errs
        |> List.filter (fun err -> err.GetType() = typeof<'Error>)
        |> List.map unbox<'Error>

    /// Iterates over the errors with a matching type
    /// and preforms the actions. Useful for side effects
    /// like logging.
    let forType action errs = errs |> ofType |> List.iter action

    let private setMessageDefaults config =
        let config =
            config
            |> Option.defaultValue defaultErrorMessageConfig

        let summaryMsg =
            config.SummaryMessage
            |> Option.defaultValue DefaultErrorMessageSummaryMessage

        let deliminator =
            config.Deliminator
            |> Option.defaultValue DefaultErrorMessageDeliminator

        let emptyMsg =
            config.EmptyMessage
            |> Option.defaultValue DefaultErrorMessageEmptyMessage

        (deliminator, summaryMsg, emptyMsg)

    /// Creates a user friendly error message for all the
    /// provided errors.
    let toMessageWithMapping config mapErrorToString (ErrorList errs) =

        let (deliminator, summaryMsg, emptyMsg) = config |> setMessageDefaults

        match errs with
        | [] -> emptyMsg
        | [ head ] -> head.ToString()
        | ls ->
            let msgs = ls |> List.map mapErrorToString

            let msg = String.Join(deliminator, msgs)

            $"{summaryMsg} {msg}"

    /// Creates a user friendly error message for all the
    /// provided errors.
    let toMessage config errList =
        errList
        |> toMessageWithMapping config (fun err -> err.ToString())
