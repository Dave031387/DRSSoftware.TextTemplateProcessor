namespace DRSSoftware.TextTemplateProcessor.Core;

/// <summary>
/// The <see cref="Messages" /> static class defines all the message strings that are used for
/// exceptions, log entries, etc, in the Text Template Processor class library.
/// </summary>
internal static class Messages
{
    // internal static string MsgAttemptingToReadFile => "Attempting to read text template file:\n{0}";
    // internal static string MsgAttemptToGenerateSegmentBeforeItWasLoaded => "An attempt was made to generate segment \"{0}\" before the template was loaded.";
    // internal static string MsgAttemptToLoadMoreThanOnce => "Attempted to load template file \"{0}\" more than once. Repeat loads will be ignored.";
    // internal static string MsgClearTheOutputDirectory => "\nCONFIRM: Do you want to clear the contents of the following directory?\n{0}";
    internal static string MsgCombinePathsArgument1IsNull => "The first argument passed to {0} must not be null.";
    internal static string MsgCombinePathsArgument2IsNull => "The second argument passed to {0} must not be null.";
    // internal static string MsgContinuationPrompt => "Press [ENTER] to continue...";
    internal static string MsgDependencyIsNull => "The {1} dependency object passed to the {0} class should not be null.";
    internal static string MsgDirectoryNotFound => "The specified directory was not found. Directory path: {0}";
    internal static string MsgDirectoryPathIsEmptyOrWhitespace => "The directory path must not be empty or contain only whitespace.";
    internal static string MsgDynamicallyGeneratedAssembliesNotSupported => "Dynamically-generated assemblies are not supported by the Text Template Processor.";
    internal static string MsgErrorCount => "  Errors: {0}";
    // internal static string MsgErrorWhenClearingOutputDirectory => "An unexpected error occurred while trying to clear the output directory. {0}";
    // internal static string MsgErrorWhenCreatingOutputDirectory => "An unexpected error occurred when creating the output directory. {0}";
    // internal static string MsgErrorWhenLocatingSolutionDirectory => "An unexpected error occurred while trying to locate the solution directory. {0}";
    // internal static string MsgErrorWhileConstructingFilePath => "An error occurred when trying to construct the output file path. {0}";
    // internal static string MsgErrorWhileReadingTemplateFile => "An error occurred while reading the template file. {0}";
    internal static string MsgFileNotFound => "The specified file was not found. Full file path: {0}";
    internal static string MsgFilePathIsEmptyOrWhitespace => "The file path must not be empty or contain only whitespace.";
    // internal static string MsgFileSuccessfullyRead => "The text template file has been successfully read.";
    internal static string MsgFirstTimeIndentHasBeenTruncated => "The calculated first time indent for segment {0} went negative. It will be set to zero.";
    internal static string MsgFirstTimeIndentIsInvalid => $"The {FirstTimeIndentOption} option value for segment \"{{0}}\" must be a number between {MinIndentValue} and {MaxIndentValue}. The value found was \"{{1}}\"";
    internal static string MsgFirstTimeIndentSetToZero => $"Found a {FirstTimeIndentOption} option value of zero for segment \"{{0}}\". This value disables the first time indent processing.";
    internal static string MsgFoundDuplicateOptionNameOnHeaderLine => "The option \"{1}\" appears more than once for segment \"{0}\". Only the first occurrence will be used.";
    // internal static string MsgFoundDuplicateSegmentName => "Segment name \"{0}\" appears more than once in the template file. Only the first occurrence will be used.";
    // internal static string MsgFoundSolutionDirectoryPath => "The solution directory path was determined to be: {0}";
    // internal static string MsgFourthCharacterMustBeBlank => "The fourth character of each template line should be blank:\n{0}\n   ^";
    // internal static string MsgFullPathCannotBeDetermined => "The full path can't be determined because the solution directory path is unknown.";
    // internal static string MsgGeneratedTextHasBeenReset => "The generated text cache for template file \"{0}\" has been reset.";
    // internal static string MsgGeneratedTextIsEmpty => "The generated text is empty. Unable to write to output file \"{0}\"";
    // internal static string MsgGeneratedTextIsNull => "Unable to write to the output file because the generated text is null.";
    internal static string MsgIndentValueMustBeValidNumber => "The specified indent value \"{0}\" is not a valid integer value.";
    internal static string MsgIndentValueOutOfRange => $"The indent value must be a number between {MinIndentValue} and {MaxIndentValue} but the specified value was \"{{0}}\".";
    // internal static string MsgInvalidControlCode => "The following template line doesn't begin with a valid control code:\n{0}\n^^^";
    internal static string MsgInvalidDirectoryCharacters => "The directory path contains invalid characters.";
    internal static string MsgInvalidFileNameCharacters => "The file name contains invalid characters.";
    internal static string MsgInvalidFormOfOption => "Segment options must follow the form \"option=value\" with no intervening spaces. Found \"{1}\" on the \"{0}\" segment header.";
    internal static string MsgInvalidPadSegmentName => $"\"{{1}}\" is not a valid name for the {PadSegmentNameOption} option for segment \"{{0}}\". It will be ignored.";
    internal static string MsgInvalidSegmentName => "\"{0}\" is not a valid segment name. The segment will be ignored.";
    internal static string MsgInvalidTabSizeOption => $"The {TabSizeOption} option value \"{{1}}\" for segment \"{{0}}\" was invalid and will be ignored.";
    internal static string MsgLeftIndentHasBeenTruncated => "The calculated line indent for segment {0} went negative. It will be set to zero.";
    // internal static string MsgLoadingTemplateFile => "Loading template file \"{0}\"";
    // internal static string MsgMinimumLineLengthInTemplateFileIs3 => "All lines in the template file must be at least 3 characters long.";
    // internal static string MsgMissingDirectoryPath => "The specified file path doesn't contain a valid directory path.";
    internal static string MsgMissingFileName => "The file name is missing from the file path.";
    // internal static string MsgMissingTokenName => "Found token start and end delimiters with no token name between them. The token will be ignored.";
    // internal static string MsgMultipleLevelsOfPadSegments => "Pad segment \"{1}\" specified for segment \"{0}\" also contains a pad segment. Multiple levels of pad segments are not allowed.";
    // internal static string MsgNextLoadRequestBeforeFirstIsWritten => "Template file \"{0}\" is being loaded before any output was written for template file \"{1}\"";
    // internal static string MsgNoTextLinesFollowingSegmentHeader => "The header line for segment \"{0}\" must be followed by one or more valid text lines. The segment will be ignored.";
    internal static string MsgNullDirectoryPath => "The directory path must not be null.";
    internal static string MsgNullFilePath => "The file path must not be null.";
    internal static string MsgOptionNameMustPrecedeEqualsSign => "An option name must appear immediately before the equals sign with no intervening spaces in the \"{0}\" segment header.";
    internal static string MsgOptionValueMustFollowEqualsSign => "The value for option \"{1}\" must appear immediately after the equals sign with no intervening spaces in the \"{0}\" segment header.";
    // internal static string MsgOutputDirectoryCleared => "The output directory has been cleared.";
    // internal static string MsgOutputDirectoryNotSet => "The output file can't be written because the output directory hasn't been set.";
    // internal static string MsgPadSegmentMustBeDefinedEarlier => "The PAD segment name \"{1}\" referenced by segment \"{0}\" must be defined earlier in the template file. It will be ignored.";
    // internal static string MsgPadSegmentNameSameAsSegmentHeaderName => "The PAD segment name and segment header name for segment \"{0}\" are identical. The PAD segment name will be ignored.";
    // internal static string MsgPathIsNotRooted => "Expected a rooted path, but found \"{0}\"";
    // internal static string MsgProcessingSegment => "Processing segment \"{0}\"...";
    internal static string MsgProcessingSummary => "Processing summary:";
    // internal static string MsgRootPathIsNull => "The root directory path must not be null.";
    // internal static string MsgSegmentHasBeenAdded => "Segment \"{0}\" has been added to the control dictionary.";
    // internal static string MsgSegmentHasBeenReset => "Segment \"{0}\" has been reset.";
    // internal static string MsgSegmentHasNoTextLines => "Tried to generate segment \"{0}\" but the segment has no text lines.";
    internal static string MsgSegmentNameIsMissing => "The segment name is missing from the segment header line. The segment will be ignored.";
    // internal static string MsgSegmentNameIsNullOrWhitespace => "The segment name passed into the GenerateSegment method was null, empty or whitespace.";
    internal static string MsgTabSizeTooLarge => "The requested tab size {0} is too large. The maximum value {1} will be used.";
    internal static string MsgTabSizeTooSmall => "The requested tab size {0} is too small. The minimum value {1} will be used.";
    internal static string MsgTabSizeValueMustBeValidNumber => "The specified tab size value \"{0}\" is not a valid integer value.";
    internal static string MsgTabSizeValueOutOfRange => $"The tab size must be an integer between {MinTabSize} and {MaxTabSize}, but the specified value was {{0}}.";
    // internal static string MsgTemplateFileIsEmpty => "This template file is empty: {0}";
    // internal static string MsgTemplateFilePathNotSet => "Unable to load the template file because a valid file path has not been set.";
    // internal static string MsgTemplateHasBeenReset => "The environment for template file \"{0}\" has been reset.";
    // internal static string MsgTokenDictionaryContainsInvalidTokenName => "The token dictionary contained an invalid token name \"{1}\" for segment \"{0}\".";
    // internal static string MsgTokenDictionaryIsEmpty => "An empty token dictionary was supplied for segment \"{0}\".";
    // internal static string MsgTokenDictionaryIsNull => "A null token dictionary was supplied for segment \"{0}\".";
    // internal static string MsgTokenEndAndTokenEscapeAreSame => "The token end delimiter \"{0}\" must not be the same as the same as the token escape character \"{1}\".";
    // internal static string MsgTokenEndDelimiterIsEmpty => "The token end delimiter must not be empty or whitespace.";
    // internal static string MsgTokenEndDelimiterIsNull => "The token end delimiter must not be null.";
    // internal static string MsgTokenHasInvalidName => "Found a token with an invalid name: \"{0}\". It will be ignored.";
    // internal static string MsgTokenMissingEndDelimiter => "Found a token start delimiter with no matching end delimiter. The token will be ignored.";
    // internal static string MsgTokenNameNotFound => "The token name \"{1}\" in segment {0} wasn't found in the token dictionary. It will be output as is.";
    // internal static string MsgTokenStartAndTokenEndAreSame => "The token start delimiter \"{0}\" must not be the same as the same as the token end delimiter \"{1}\".";
    // internal static string MsgTokenStartAndTokenEscapeAreSame => "The token start delimiter \"{0}\" must not be the same as the same as the token escape character \"{1}\".";
    // internal static string MsgTokenStartDelimiterIsEmpty => "The token start delimiter must not be empty or whitespace.";
    // internal static string MsgTokenStartDelimiterIsNull => "The token start delimiter must not be null.";
    // internal static string MsgTokenStartDelimiterWarning => "Ending the token start delimiter with '-', '+' or '=' may cause confusion and lead to unexpected errors.";
    // internal static string MsgTokenValueIsEmpty => "Found token \"{1}\" with no assigned value while generating segment \"{0}\".";
    // internal static string MsgTokenWithEmptyValue => "Token \"{1}\" was passed in with an empty value for segment \"{0}\".";
    // internal static string MsgTokenWithNullValue => "Token \"{1}\" was passed in with a null value for segment \"{0}\".";
    internal static string MsgUnableToClearDirectory => "An unexpected exception occurred while clearing directory \"{0}\"";
    internal static string MsgUnableToCombineFilePaths => "An unexpected exception occurred while attempting to combine file paths \"{0}\" and \"{1}\"";
    internal static string MsgUnableToCreateDirectory => "An unexpected exception occurred while attempting to create directory \"{0}\"";
    // internal static string MsgUnableToCreateOutputDirectory => "Encountered an error when trying to create the output directory path.\n{0}";
    // internal static string MsgUnableToGenerateSegment => "Unable to generate segment \"{0}\".\nReason: {1}";
    internal static string MsgUnableToGetFullPathString => "An unexpected exception occurred while trying to determine the full path string for path \"{0}\"";
    // internal static string MsgUnableToGetUserResponse => "Unable to get the user response.\nReason: {0}";
    // internal static string MsgUnableToLoadTemplateFile => "Unable to load template file \"{0}\".\nReason: {1}";
    internal static string MsgUnableToLocateSolutionDirectory => "The directory containing the solution file could not be found.";
    internal static string MsgUnableToReadTextFile => "An unexpected exception occurred while trying to read from file path \"{0}\"";
    // internal static string MsgUnableToResetAll => "Unable to perform Reset All.\nReason{0}";
    // internal static string MsgUnableToResetGeneratedText => "Unable to reset the generated text buffer.\nReason: {0}";
    // internal static string MsgUnableToResetSegment => "Unable to reset segment \"{0}\".\nReason: {1}";
    // internal static string MsgUnableToResetUnknownSegment => "Unable to reset segment \"{0}\" because of a null, empty or unknown segment name.";
    // internal static string MsgUnableToSetTemplateFilePath => "Unable to set the template file path \"{0}\".Reason: {1}";
    // internal static string MsgUnableToWriteFile => "Unable to write to output file. {0}";
    // internal static string MsgUnableToWriteGeneratedTextToFile => "Unable to write generated text to file.\nReason: {0}";
    internal static string MsgUnableToWriteToTextFile => "An unexpected exception occurred while attempting to write to text file \"{0}\"";
    internal static string MsgUncPathIsNotSupported => "UNC paths are not supported in this version of the Text Template Processor. The specified path was \"{0}\"";
    // internal static string MsgUnknownSegmentName => "A request was made to generate segment \"{0}\" but that segment wasn't found in the template file.";
    internal static string MsgUnknownSegmentOptionFound => "An unknown segment option \"{1}\" was found on segment \"{0}\". It will be ignored.";
    // internal static string MsgUnknownTokenName => "An unknown token name \"{1}\" was supplied for segment \"{0}\". It will be ignored.";
    internal static string MsgWarningCount => "  Warnings: {0}";
    // internal static string MsgWritingTextFile => "Writing generated text to file \"{0}\"";
    // internal static string MsgYesNoPrompt => "Enter Y (yes) or N (no)...";

    /// <summary>
    /// Format the given <paramref name="message"/> composite string by replacing each format item with the given <paramref name="strings"/>.
    /// </summary>
    /// <param name="message">
    /// A composite string containing zero or more format items which are to be replaced by the strings contained in <paramref name="strings"/>.
    /// </param>
    /// <param name="strings">
    /// An array of string values to be substituted for the corresponding format items found in <paramref name="message"/>.
    /// </param>
    /// <returns>
    /// The formatted version of <paramref name="message"/> having all format items replaced with the appropriate string values.
    /// </returns>
    internal static string FormatMessage(string message, params string?[] strings)
    {
        strings ??= [null];

        for (int i = 0; i < strings.Length; i++)
        {
            strings[i] ??= NullStringValue;
        }

        return HasFormatItems(message)
            ? string.Format(message, strings)
            : message;
    }
}