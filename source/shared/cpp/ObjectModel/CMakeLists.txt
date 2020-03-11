cmake_minimum_required(VERSION 3.16)

# set the project name
project(ObjectModel)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED True)

add_library(ObjectModel OBJECT   BaseElement.cpp
  ActionParserRegistration.cpp
  ActionSet.cpp
  AdaptiveBase64Util.cpp
  AdaptiveCardParseException.cpp
  AdaptiveCardParseWarning.cpp
  BackgroundImage.cpp
  BaseActionElement.cpp
  BaseCardElement.cpp
  BaseInputElement.cpp
  ChoiceInput.cpp
  ChoiceSetInput.cpp
  CollectionTypeElement.cpp
  Column.cpp
  ColumnSet.cpp
  Container.cpp
  DateInput.cpp
  DateTimePreparsedToken.cpp
  DateTimePreparser.cpp
  ElementParserRegistration.cpp
  Enums.cpp
  Fact.cpp
  FactSet.cpp
  FeatureRegistration.cpp
  HostConfig.cpp
  Image.cpp
  ImageSet.cpp
  Inline.cpp
  MarkDownBlockParser.cpp
  MarkDownHtmlGenerator.cpp
  MarkDownParsedResult.cpp
  MarkDownParser.cpp
  Media.cpp
  MediaSource.cpp
  NumberInput.cpp
  OpenUrlAction.cpp
  ParseContext.cpp
  ParseResult.cpp
  ParseUtil.cpp
  RichTextBlock.cpp
  RichTextElementProperties.cpp
  SemanticVersion.cpp
  Separator.cpp
  SharedAdaptiveCard.cpp
  ShowCardAction.cpp
  SubmitAction.cpp
  TextBlock.cpp
  TextElementProperties.cpp
  TextInput.cpp
  TextRun.cpp
  TimeInput.cpp
  ToggleInput.cpp
  ToggleVisibilityAction.cpp
  ToggleVisibilityTarget.cpp
  UnknownAction.cpp
  UnknownElement.cpp
  Util.cpp
  jsoncpp.cpp)

target_precompile_headers(ObjectModel
  PUBLIC
  pch.h)

