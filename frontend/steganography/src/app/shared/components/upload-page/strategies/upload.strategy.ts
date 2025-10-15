import { Type } from "@angular/core"
import { DecryptComponentComponent } from "../decrypt-component/decrypt-component.component"
import { EncryptComponentComponent } from "../encrypt-component/encrypt-component.component"
import { UploadStrategyComponent } from "./upload-strategy.model"

const uploadStrategyComponentMap = new Map<string, Type<UploadStrategyComponent>>([
    ['encrypt', EncryptComponentComponent],
    ['decrypt', DecryptComponentComponent]
])

export class UploadStrategyFactory {
    static getStrategyComponent(name: string): Type<UploadStrategyComponent> {
        const component = uploadStrategyComponentMap.get(name);
        if (!component) throw new Error('Invalid upload strategy name.');

        return component;
    }
}