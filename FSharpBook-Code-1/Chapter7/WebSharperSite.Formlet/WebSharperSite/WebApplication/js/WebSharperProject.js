// p34d97297c72e8629

if (typeof WebSharperProject$ == "undefined")
{
  WebSharperProject$ = {};
}

WebSharperProject$.$MainPage =
function ()
{};

(WebSharperProject$.$MainPage).prototype = new W$.$Control();

((WebSharperProject$.$MainPage).prototype).get_Body =
function (unitVar1)
{
  var _this = this;
  var x = new W$.$List({
                         $: 1,
                         $0: ((W$.Html).text)(""),
                         $1: new W$.$List({
                                            $: 0
                                          })
                       });
  var body = x;
  var name = "p";
  var res = ((W$.Html).Tag)(name, body);
  var f =
      function (n)
      {
        return (res.set_Text)(
               ((W$.Rpc).Sync)(
               "WebSharperProject.Application, WebSharperProject, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null:GetPrimes",
               [n]));
      };
  var label = "Enter max number:";
  var x_1 =
      new W$.$List({
                     $: 1,
                     $0:
                     (function (formlet)
                      {
                        return ((((IntelliFactory$.WebSharper$).Formlet$).Formlet$).Run)(
                               f, formlet);
                      })(
                     (function (formlet)
                      {
                        return ((((IntelliFactory$.WebSharper$).Formlet$).Enhance$).WithFormContainer)(
                               formlet);
                      })(
                     (function (formlet)
                      {
                        return ((((IntelliFactory$.WebSharper$).Formlet$).Enhance$).WithSubmitAndResetButtons)(
                               formlet);
                      })(
                     (function (formlet)
                      {
                        return ((((IntelliFactory$.WebSharper$).Formlet$).Enhance$).WithValidationIcon)(
                               formlet);
                      })(
                     (function (flet)
                      {
                        return ((((IntelliFactory$.WebSharper$).Formlet$).Enhance$).WithTextLabel)(
                               label, flet);
                      })(
                     (function (builder_)
                      {
                        return (builder_.Bind)(
                               (((((IntelliFactory$.WebSharper$).Formlet$).Validator$).IsInt)(
                                "Must be int"))(
                               ((((IntelliFactory$.WebSharper$).Formlet$).Controls$).Input)(
                               "100")),
                               function (_arg1)
                               {
                                 var max = _arg1;
                                 return (builder_.Return)(
                                        (function (value)
                                         {
                                           return parseInt(value);
                                         })(max));
                               });
                      })(
                     (((((IntelliFactory$.WebSharper$).Formlet$).Operators$).formlet).get)(
                     ))))))),
                     $1: new W$.$List({
                                        $: 1,
                                        $0: res,
                                        $1: new W$.$List({
                                                           $: 0
                                                         })
                                      })
                   });
  var body_1 = x_1;
  var name_1 = "div";
  return ((W$.Html).Tag)(name_1, body_1);
};

((((W$.Registry).Types).get)()).MainPage_n624d6a77cbc87721 =
WebSharperProject$.$MainPage;
((WebSharperProject$.$MainPage).prototype).$$IComponent_p4642b924f95be4a6__IComponent
= 1;
((WebSharperProject$.$MainPage).prototype).$$IDisposable_p1a5dece2430d1c7__IDisposable
= 1;
((WebSharperProject$.$MainPage).prototype).$$IParserAccessor_p23114829d1dcea79__IParserAccessor
= 1;
((WebSharperProject$.$MainPage).prototype).$$IUrlResolutionService_p3d8fd69f2d090451__IUrlResolutionService
= 1;
((WebSharperProject$.$MainPage).prototype).$$IDataBindingsAccessor_naf467a036212eec__IDataBindingsAccessor
= 1;
((WebSharperProject$.$MainPage).prototype).$$IControlBuilderAccessor_p12767bfcffc685b4__IControlBuilderAccessor
= 1;
((WebSharperProject$.$MainPage).prototype).$$IControlDesignerAccessor_p21c9dfd732eeac7e__IControlDesignerAccessor
= 1;
((WebSharperProject$.$MainPage).prototype).$$IExpressionsAccessor_p63d69e724f7de2af__IExpressionsAccessor
= 1;
((WebSharperProject$.$MainPage).prototype).$$IWidget_p4036373d8372c14d__IWidget
= 1;
((WebSharperProject$.$MainPage).prototype).$$INode_p5129303de1c6d8ef__INode = 1;
