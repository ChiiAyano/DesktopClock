applyTo: **/*.cs
name: C# Instructions
description: Instructions for C# code generation
---

# C# Instructions
- このプロジェクトにおける C# は、一般的なベストプラクティスに従い記述します。
- コーディングガイドラインは Microsoft の公式ドキュメントに準拠します。
  - 参考: https://learn.microsoft.com/ja-jp/dotnet/csharp/fundamentals/coding-style/coding-conventions
- 別途ルールとして以下の通り定めます
  - 名前空間はプロジェクト名に従い命名します。ディレクトリ内にある場合はディレクトリ名を名前空間に含めます。
  - クラス、メソッド、プロパティ、フィールド上のデリゲートは PascalCase を使用します。
  - プライベートフィールドは `_camelCase` を使用します。
  - 非公開メンバーには明示的にアクセス修飾子を付与します。
  - 定数は `const` または `static readonly` を使用し、PascalCase を使用します。
  - インターフェース名は `I` で始め、PascalCase を使用します。
  - 非同期メソッドには `Async` サフィックスを付与します。
  - 型推論は左辺値では可能な限りすべて `var` を使用します。右辺値での new 式は左辺値で型が明確な場合にのみ使用します。
