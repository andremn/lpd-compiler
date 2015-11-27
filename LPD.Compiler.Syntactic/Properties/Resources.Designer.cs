﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LPD.Compiler.Syntactic.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LPD.Compiler.Syntactic.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Já existe uma função chamada &apos;{2}&apos;..
        /// </summary>
        internal static string DoubleFuncErrorMessage {
            get {
                return ResourceManager.GetString("DoubleFuncErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Identificador com o nome &apos;{2}&apos; já foi declarado..
        /// </summary>
        internal static string DoubleIdentificatorErrorMessage {
            get {
                return ResourceManager.GetString("DoubleIdentificatorErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Já existe um procedimento chamado &apos;{2}&apos;..
        /// </summary>
        internal static string DoubleProcErrorMessage {
            get {
                return ResourceManager.GetString("DoubleProcErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Esperado vírgula ou dois pontos, mas o símbolo &apos;{2}&apos; foi encontrado..
        /// </summary>
        internal static string ExpectedCommaOrPointsErrorMessage {
            get {
                return ResourceManager.GetString("ExpectedCommaOrPointsErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Função.
        /// </summary>
        internal static string Function {
            get {
                return ResourceManager.GetString("Function", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: {2}..
        /// </summary>
        internal static string GenericErrorMessage {
            get {
                return ResourceManager.GetString("GenericErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Tipo &apos;{2}&apos; não pode ser atribuido a variável do tipo &apos;{3}&apos;..
        /// </summary>
        internal static string IncompatibleAttributionErrorMessage {
            get {
                return ResourceManager.GetString("IncompatibleAttributionErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Esperado expressão do tipo &apos;booleano&apos; dentro de um comando &apos;{2}&apos;..
        /// </summary>
        internal static string IncompatibleCommandExpressionErrorMessage {
            get {
                return ResourceManager.GetString("IncompatibleCommandExpressionErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Termo inválido..
        /// </summary>
        internal static string InvalidTerm {
            get {
                return ResourceManager.GetString("InvalidTerm", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Simbolo &apos;{2}&apos; invalido..
        /// </summary>
        internal static string InvalidTokenErrorMessage {
            get {
                return ResourceManager.GetString("InvalidTokenErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Nem todos os caminhos retornam um valor na função &apos;{2}&apos;..
        /// </summary>
        internal static string MissingFunctionReturnMessage {
            get {
                return ResourceManager.GetString("MissingFunctionReturnMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Encontrado &quot;fim&quot; sem &quot;inicio&quot;..
        /// </summary>
        internal static string MissingInicioErrorMessage {
            get {
                return ResourceManager.GetString("MissingInicioErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}:  &apos;{2}&apos; não é uma função ou uma variável..
        /// </summary>
        internal static string NotAFuncVarErrorMessage {
            get {
                return ResourceManager.GetString("NotAFuncVarErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Função &apos;{2}&apos; não declarada..
        /// </summary>
        internal static string NotFoundFuncErrorMessage {
            get {
                return ResourceManager.GetString("NotFoundFuncErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Identificador &apos;{2}&apos; não declarado..
        /// </summary>
        internal static string NotFoundIdentifierErrorMessage {
            get {
                return ResourceManager.GetString("NotFoundIdentifierErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Procedimento &apos;{2}&apos; não declarado..
        /// </summary>
        internal static string NotFoundProcErrorMessage {
            get {
                return ResourceManager.GetString("NotFoundProcErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Procedimento.
        /// </summary>
        internal static string Procedure {
            get {
                return ResourceManager.GetString("Procedure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Fim inesperado do arquivo. Não está faltando um ponto depois do &apos;fim&apos;?.
        /// </summary>
        internal static string UnexpectedEndOfFileErrorMessage {
            get {
                return ResourceManager.GetString("UnexpectedEndOfFileErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Símbolo &apos;{2}&apos; inesperado na expressão..
        /// </summary>
        internal static string UnexpectedOperatorInExpression {
            get {
                return ResourceManager.GetString("UnexpectedOperatorInExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Esperado {2}..
        /// </summary>
        internal static string UnexpectedTokenErrorMessage {
            get {
                return ResourceManager.GetString("UnexpectedTokenErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Linha {0}, coluna {1}: Uma função deve estar dentro de uma expressão..
        /// </summary>
        internal static string WrongFunctionCall {
            get {
                return ResourceManager.GetString("WrongFunctionCall", resourceCulture);
            }
        }
    }
}
